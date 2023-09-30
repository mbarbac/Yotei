namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Provides the ability of generating code that assigns to a given variable a new instance of
/// the associated type, taking into consideration a set of optional specifications for doing so,
/// and an optional enforced value to be assign to a given member of the type while building it.
/// </summary>
internal partial class TypeBuilder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="context"></param>
    public TypeBuilder(ITypeSymbol symbol, SourceProductionContext context)
    {
        Symbol = symbol.ThrowWhenNull(nameof(symbol));

        if (symbol.IsNamespace)
        {
            context.ErrorTypeIsNamespace(symbol);
            throw new ArgumentException("Symbol is namespace.").WithData(symbol, nameof(symbol));
        }
        if (Symbol is not INamedTypeSymbol named)
        {
            context.ErrorTypeNotNamed(Symbol);
            throw new ArgumentException("Type is not named.").WithData(symbol, nameof(symbol));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Builder: {Symbol.Name}";

    /// <summary>
    /// The type for which the code of generating a new instance will be emitted.
    /// </summary>
    public ITypeSymbol Symbol { get; }

    /// <summary>
    /// The context provided to this instance.
    /// </summary>
    public SourceProductionContext Context { get; }

    // ----------------------------------------------------

    string Receiver = default!;
    BuilderSpecs BuilderSpecs = default!;
    EnforcedMember? EnforcedMember = null;

    List<IMethodSymbol> TypeMethods = default!;
    List<IPropertySymbol> TypeProperties = default!;
    List<IFieldSymbol> TypeFields = default!;

    /// <summary>
    /// Returns the code that assigns to the 'receiver' variable a new instance of the associated
    /// type, according to the given specifications, and optionally replacing the original value
    /// of the enforced member with the provided one. Otherwise, returns null if the requested
    /// code cannot be generated.
    /// </summary>
    /// <param name="receiver">
    /// The name of the receiver variable where to assign the code to generate a new instance.
    /// </param>
    /// <param name="specs">
    /// If not null, the specifications that describe how to obtain a new instance of the host
    /// type, with the format '[name][(argument)][optionals]', where:
    /// <br/>-- [name]: the name of the builder method(s) to consider. If it is null, then only
    /// the type constructors will be taken into consideration. Otherwise, the method(s) must
    /// return an instance of the associated type.
    /// <br/> Some generators may accept special names (as 'base' or 'this') to indicate that
    /// alternate ways shall be used.
    /// <br/>-- [(arguments)]: the comma-separated list of builder arguments. If not used, or if
    /// it is '(*)', then all filtered builders are taken into consideration despite their actual
    /// arguments. If empty '()', then only the parameterless ones will be considered. Otherwise,
    /// the specifications wth the format '[name][=@|member[!]]', where:
    /// <br/>> [name]: the name of the builder argument.
    /// <br/>> [=@|member]: the source from which to obtain the value of the argument. If it is
    /// not used, then a member with a matching name will be used. If '=@', then the name of the
    /// enforced variable will be used.
    /// <br/>> [!]: to use a clone of the value, instead of its regular value.
    /// <br/>-- [optionals]: an optional comma-separated chain of optional init/set arguments to
    /// use (provided they have not been yet used in a given builder). If [optionals] is not used,
    /// it is equivalent to '+*'. Otherwise, their format is '[+|-][*|spec]' where:
    /// <br/>> [+|-] controls whether it is an include or exclude specification
    /// <br/>> [*] indicates that all remanining members are affected (and erasing any previous
    /// optional specification).
    /// <br/>> [spec] follows the same rules as the argument ones.
    /// </param>
    /// <param name="enforced">
    /// If not null, the specification of the enforced member whose value will be replaced while
    /// generating the new instance of the associated type.
    /// </param>
    /// <returns></returns>
    public string? GetCode(string receiver, string? specs = null, EnforcedMember? enforced = null)
    {
        Receiver = receiver.NotNullNotEmpty(nameof(receiver));
        BuilderSpecs = new BuilderSpecs(specs);
        EnforcedMember = enforced;
        string? code;

        CaptureTypeBuilders(); if (TypeMethods.Count == 0) return null;
        CaptureTypeProperties();
        CaptureTypeFields();

        code = TryCopyBuilders(true); if (code != null) return code;
        code = TryCopyBuilders(false); if (code != null) return code;
        code = TryRegularBuilders(); if (code != null) return code;
        code = TryEmptyBuilders(); if (code != null) return code;

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries empty builders.
    /// </summary>
    /// <returns></returns>
    public string? TryEmptyBuilders()
    {
        foreach (var method in TypeMethods)
        {
            if (method.Parameters.Length != 0) continue;

            var isConstructor = method.MethodKind == MethodKind.Constructor;
            var name = isConstructor ? $"new {Symbol.Name}" : method.Name;
            var code = $"{name}()";

            var properties = isConstructor ? GetWriteProperties(TypeProperties) : new();
            var fields = isConstructor ? GetWriteFields(TypeFields) : new();

            return CodeAndInits(code, isConstructor, properties, fields);
        }

        // Not found...
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries copy builders.
    /// </summary>
    /// <param name="strict"></param>
    /// <returns></returns>
    public string? TryCopyBuilders(bool strict)
    {
        foreach (var method in TypeMethods)
        {
            if (method.Parameters.Length != 1) continue;

            var par = method.Parameters[0];
            if (!IsValid(par, strict)) continue;

            var isConstructor = method.MethodKind == MethodKind.Constructor;
            var name = isConstructor ? $"new {Symbol.Name}" : method.Name;
            var code = $"{name}(this)";

            var properties = isConstructor ? GetWriteProperties(TypeProperties) : new();
            var fields = isConstructor ? GetWriteFields(TypeFields) : new();

            return CodeAndInits(code, isConstructor, properties, fields);
        }

        // Not found...
        return null;

        // Determines if the parameter is compatible or not.
        bool IsValid(IParameterSymbol par, bool strict)
        {
            return strict
                ? SymbolEqualityComparer.Default.Equals(Symbol, par.Type)
                : Symbol.IsAssignableTo(par.Type);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries regular builders.
    /// </summary>
    /// <returns></returns>
    public string? TryRegularBuilders()
    {
        foreach (var method in TypeMethods)
        {
            if (method.Parameters.Length == 0) continue;

            var all = BuilderSpecs.Arguments.Count == 1 && BuilderSpecs.Arguments[0].Name == "*";
            if (!all && method.Parameters.Length != BuilderSpecs.Arguments.Count) continue;

            var properties = TypeProperties.ToList();
            var fields = TypeFields.ToList();

            var isConstructor = method.MethodKind == MethodKind.Constructor;
            var name = isConstructor ? $"new {Symbol.Name}" : method.Name;
            var sb = new StringBuilder($"{name}(");

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                var par = method.Parameters[i];

                // All parameters to be considered...
                if (all)
                {
                    var member = MatchMember(par.Name, true, out var error, properties, fields);
                    if (error) Context.WarningCannotGenerateCode(EnforcedMember?.Symbol ?? Symbol);
                    if (member == null) return null;

                    if (i > 0) sb.Append(", ");
                    sb.Append(member.Name);
                }

                // Only the parameters specified...
                else
                {
                }
            }

            sb.Append(")");
            var code = sb.ToString();
            properties = GetWriteProperties(properties);
            fields = GetWriteFields(fields);

            return CodeAndInits(code, isConstructor, properties, fields);
        }

        // Not found...
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Adds to the given code the collection of remaining init/set members, if any.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="isConstructor"></param>
    /// <param name="properties"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    string CodeAndInits(
        string code, bool isConstructor,
        List<IPropertySymbol> properties, List<IFieldSymbol> fields)
    {
        return null!;
    }
}
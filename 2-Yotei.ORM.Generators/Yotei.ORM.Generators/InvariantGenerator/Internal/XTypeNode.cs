namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        ReturnType = Symbol;
        ReturnNullable = false;
        ReturnOptions = EasyNameOptions.Default;
        Arity = 0;
        KType = null!; KTypeNullable = false; KTypeName = null!;
        TType = null!; TTypeNullable = false; TTypeName = null!;
        Template = null!;
        Bracket = null!;
    }
    INamedTypeSymbol ReturnType;
    bool ReturnNullable;
    EasyNameOptions ReturnOptions;
    int Arity;
    INamedTypeSymbol KType; bool KTypeNullable; string KTypeName;
    INamedTypeSymbol TType; bool TTypeNullable; string TTypeName;
    Type Template;
    string Bracket;

    const string TheNamespace = "Yotei.ORM.Tools";
    const string IListName = "IInvariantList";
    const string ListName = "InvariantList";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;

        if (Symbol.IsRecord) { CoreDiagnostics.RecordsNotSupported(Symbol).Report(context); return false; }
        if (Attributes.Length == 0) { CoreDiagnostics.NoAttributes(Symbol).Report(context); return false; }
        if (Attributes.Length > 1) { CoreDiagnostics.TooManyAttributes(Symbol).Report(context); return false; }
        
        var at = Attributes[0];
        var atc = at.AttributeClass;
        if (atc is null) { CoreDiagnostics.InvalidAttribute(Symbol).Report(context); return false; }

        // Getting the return type, if any is requested...
        if (GetReturnType(at, out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(Symbol, type);
            if (!same) ReturnOptions = EasyNameOptions.Full with
            { TypeUseNullable = false };
        }

        // Attribute uses no generic arguments...
        if (atc.Arity == 0)
        {
            var args = at.ConstructorArguments
                .Where(x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            if (args.Length == 1) // One type argument: <T>...
            {
                Template = typeof(IChainTemplate<>);
                Arity = 1;
                TType = args[0].UnwrapNullable(out TTypeNullable);
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{TTypeName}>";
            }

            if (args.Length == 2) // Two type arguments: <K, T>...
            {
                Template = typeof(IChainTemplate<,>);
                Arity = 2;
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(EasyNameOptions.Full) + (KTypeNullable ? "?" : "");
                TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
                Bracket = $"<{KTypeName}, {TTypeName}>";
            }
        }

        // Attribute uses one generic argument: <T>...
        if (atc.Arity == 1)
        {
            Template = typeof(IChainTemplate<>);
            Arity = 1;
            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{TTypeName}>";
        }

        // Attribute uses two generic arguments: <K, T>...
        if (atc.Arity == 2)
        {
            Template = typeof(IChainTemplate<,>);
            Arity = 2;
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(EasyNameOptions.Full) + (KTypeNullable ? "?" : "");
            TTypeName = TType.EasyName(EasyNameOptions.Full) + (TTypeNullable ? "?" : "");
            Bracket = $"<{KTypeName}, {TTypeName}>";
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override string GetHeader(SourceProductionContext context)
    {
        var atr = Attributes[0].AttributeClass!.Name;
        var name = atr.RemoveTail("Attribute").ToString();

        var xtra = $"{TheNamespace}.{name}{Bracket}";
        var head = base.GetHeader(context) + $" : {xtra}";
        return head;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <param name="needNL"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Iterating through the template methods...
        var existing = Symbol.GetMembers().OfType<IMethodSymbol>().ToArray();
        var methods = Template.GetMembers().OfType<MethodInfo>().Where(x => x.DeclaringType == Template);

        foreach (var method in methods)
        {
            if (!CanEmit(method)) continue;
        }

        /// <summary>
        /// Determines if the given method can be emitted, or not.
        /// </summary>
        bool CanEmit(MethodInfo method)
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a method with the given name and argument types, either in the given type
    /// or in the first one in the given chains. In strict mode, the argument types must match.
    /// Otherwise, it is just enough if they are assignable.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="usehost"></param>
    /// <param name="name"></param>
    /// <param name="argtypes"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool FindMethod(
        INamedTypeSymbol type,
        bool usehost,
        string name,
        ITypeSymbol[] argtypes,
        bool strict,
        [NotNullWhen(true)] out IMethodSymbol? value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        name = name.NotNullNotEmpty(true);
        argtypes.ThrowWhenNull();
        chains.ThrowWhenNull();

        var found = type.Finder(usehost, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == name &&
                x.Parameters.Length == argtypes.Length &&
                argtypes
                    .Select((t, i) => new { T = t, I = i })
                    .All(arg => strict ?
                        SymbolEqualityComparer.Default.Equals(arg.T, x.Parameters[arg.I].Type) :
                        arg.T.IsAssignableTo(x.Parameters[arg.I].Type)));

            return value is not null;
        },
        out value, chains);
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value of the <see cref="IInvariantListAttribute.ReturnType"/>
    /// setting from the given attribute.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static bool GetReturnType(
        AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        var name = nameof(IInvariantListAttribute.ReturnType);

        if (at.GetNamedArgument(name, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
            {
                value = temp.UnwrapNullable(out nullable);
                return true;
            }
        }

        value = null;
        nullable = false;
        return false;
    }
}
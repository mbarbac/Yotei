namespace Yotei.Tools.WithGenerator;

// ========================================================
// T: IPropertySymbol or IFieldSymbol
internal interface IXNode<T> where T : ISymbol
{
    public T Symbol { get; }
}

// ========================================================
internal static class XNode
{
    const string USEVIRTUAL = "UseVirtual";
    const string RETURNTYPE = "ReturnType";

    extension<T>(IXNode<T> node) where T : ISymbol
    {
        public string MemberName => node.Symbol.Name;

        public string MethodName => $"With{node.MemberName}";

        public ITypeSymbol MemberType => node.Symbol switch
        {
            IPropertySymbol item => item.Type,
            IFieldSymbol item => item.Type,
            _ => throw new UnExpectedException()
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the version of this generator for documentation purposes.
    /// </summary>
    public static string DocVersion => Assembly.GetExecutingAssembly().GetName().Version.To3String();

    /// <summary>
    /// Gets the string that emits the attribute decoration, for documentation purposes.
    /// </summary>
    public static string DocAttribute => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{DocVersion}}")]
        """;

    /// <summary>
    /// Emits appropriate documentation for the generated methods.
    /// </summary>
    /// <param name="cb"></param>
    /// <param name="name"></param>
    public static void EmitDocumentation(CodeBuilder cb, string name) => cb.AppendLine($$"""
            /// <summary>
            /// Emulates the '<see langword="with"/>' keyword for the '{{name}}' member.
            /// </summary>
            {{DocAttribute}}
            """);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is decorated with a <see cref="WithAttribute"/>-alike
    /// attribute, If so, returns the found ones in the out argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static bool HasWithAttribute(
        this ISymbol symbol,
        out IEnumerable<AttributeData> attributes)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        attributes = symbol.GetAttributes([typeof(WithAttribute), typeof(WithAttribute<>)]);
        return attributes.Any();
    }

    /// <summary>
    /// Determines if the given symbol is decorated with a <see cref="InheritsWithAttribute"/>-alike
    /// attribute, If so, returns the found ones in the out argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static bool HasInheritsWithAttribute(
        this ISymbol symbol,
        out IEnumerable<AttributeData> attributes)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        attributes = symbol.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]);
        return attributes.Any();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="WithAttribute.UseVirtual"/> setting on the
    /// given attribute. If so, returns it in the out argument.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool HasUseVirtual(this AttributeData at, out bool value)
    {
        ArgumentNullException.ThrowIfNull(at);

        if (at.FindNamedArgument(USEVIRTUAL, out var arg) &&
                !arg.Value.IsNull &&
                arg.Value.Value is bool temp)
        {
            value = temp;
            return true;
        }

        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the value of the <see cref="CloneableAttribute.ReturnType"/> setting on the
    /// given attribute. If so, returns it in the out argument, and whether it is a nullable one
    /// or not.
    /// </summary>
    /// <param name="at"></param>
    /// <param name="value"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static bool HasReturnType(
        this AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at);
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        // Non-generic attribute...
        if (at.AttributeClass.Arity == 0)
        {
            if (at.FindNamedArgument(RETURNTYPE, out var arg) &&
                !arg.Value.IsNull &&
                arg.Value.Value is INamedTypeSymbol temp)
            {
                value = temp.UnwrapNullable(out nullable);
                return true;
            }
        }

        // Generic attribute...
        else if (at.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)at.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out nullable);
            return true;
        }

        // Invalid arity...
        else throw new ArgumentException("Invalid arity.").WithData(at);

        // Not found...
        value = null;
        nullable = false;
        return false;
    }

    /// <summary>
    /// Gets the appropriate options to emit the given type, based upon whether it is the same as
    /// the other given one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static EasyTypeOptions GetReturnOptions(
        this INamedTypeSymbol type,
        INamedTypeSymbol other)
    {
        var options = EasyTypeOptions.Default with
        {
            NullableStyle = EasyNullableStyle.None,
            GenericListOptions = EasyTypeOptions.Default.WithRecursive(
                namespaceStyle: EasyNamespaceStyle.Default,
                useHost: true,
                useSpecialNames: true,
                nullableStyle: EasyNullableStyle.None)
        };

        var same = SymbolEqualityComparer.Default.Equals(type, other);
        if (!same)
        {
            options.NamespaceStyle = EasyNamespaceStyle.Default;
            options.UseHost = true;
        }

        return options;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a method with the given specifications in either the given host type, if not
    /// null, or in any of the types in the given chains, in order. If found, returns it in the out
    /// argument.
    /// </summary>
    public static bool TryFindMethod(
        string methodname, ITypeSymbol argtype,
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
        [NotNullWhen(true)] out IMethodSymbol? value)
    {
        return Finder.Find(type, chains, out value, (type, out value) =>
        {
            value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                x.Name == methodname &&
                x.Parameters.Length == 1 &&
                argtype.IsAssignableTo(x.Parameters[0].Type));

            return value != null;
        });
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find a decorated member with the given specifications in either the given host
    /// type, if not null, or in any of the types in the given chains, in order. If found, returns
    /// it in the out argument as well as the decorating attribute.
    /// </summary>
    public static bool TryFindMember<T>(
        string membername,
        INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
        [NotNullWhen(true)] out T? value,
        [NotNullWhen(true)] out AttributeData? at) where T : ISymbol
    {
        var found = Finder.Find(type, chains, out (T? Member, AttributeData Attr) info,
            (type, out info) =>
            {
                var member = type.GetMembers().OfType<T>().FirstOrDefault(x => x.Name == membername);
                if (member != null &&
                member.HasWithAttribute(out var atts))
                {
                    info = new(member, atts.First());
                    return true;
                }

                info = new();
                return false;
            });

        value = found ? info.Member : default;
        at = found ? info.Attr : default;
        return found;
    }
}
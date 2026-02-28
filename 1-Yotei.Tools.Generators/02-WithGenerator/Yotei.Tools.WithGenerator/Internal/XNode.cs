namespace Yotei.Tools.WithGenerator;

// ========================================================
internal interface IXNode<T> where T : ISymbol
{
    public T Symbol { get; }
}

// ========================================================
internal static class XNode
{
    const string USEVIRTUAL = "UseVirtual";
    const string RETURNTYPE = "ReturnType";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is decorated with <see cref="WithAttribute"/> or with
    /// <see cref="WithAttribute{T}"/> attributes. If so, returns them in the out argument.
    /// </summary>
    public static bool HasWithAttributes(
        this ISymbol type,
        out IEnumerable<AttributeData> ats)
    {
        ArgumentNullException.ThrowIfNull(type);

        ats = type.GetAttributes([typeof(WithAttribute), typeof(WithAttribute<>)]);
        return ats.Any();
    }

    /// <summary>
    /// Determines if the given symbol is decorated with a <see cref="WithAttribute"/> or with
    /// a <see cref="WithAttribute{T}"/> attribute. If so, returns the first found in the out
    /// argument.
    /// </summary>
    public static bool HasWithAttribute(
        this ISymbol type,
        [NotNullWhen(true)] out AttributeData? at)
    {
        var found = type.HasWithAttributes(out var ats);
        at = found ? ats.First() : null;
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is decorated with <see cref="InheritsWithAttribute"/> or
    /// with <see cref="InheritsWithAttribute{T}"/> attributes. If so, returns them in the out
    /// argument.
    /// </summary>
    public static bool HasInheritsWithAttributes(
        this INamedTypeSymbol type,
        out IEnumerable<AttributeData> ats)
    {
        ArgumentNullException.ThrowIfNull(type);

        ats = type.GetAttributes([typeof(InheritsWithAttribute), typeof(InheritsWithAttribute<>)]);
        return ats.Any();
    }

    /// <summary>
    /// Determines if the given symbol is decorated with a <see cref="InheritsWithAttribute"/> or
    /// with a <see cref="InheritsWithAttributeAttribute{T}"/> attribute. If so, returns the firs
    /// found in the out argument.
    /// </summary>
    public static bool HasInheritsWithAttribute(
        this INamedTypeSymbol type,
        [NotNullWhen(true)] out AttributeData? at)
    {
        var found = type.HasInheritsWithAttributes(out var ats);
        at = found ? ats.First() : null;
        return found;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given attribute carries a 'UseVirtual' setting. If so, returns it in
    /// the out argument.
    /// </summary>
    public static bool HasUseVirtual(
        this AttributeData at,
        out bool value)
    {
        ArgumentNullException.ThrowIfNull(at);

        if (at.FindNamedArgument(USEVIRTUAL, out var arg))
        {
            if (!arg.Value.IsNull && arg.Value.Value is bool temp)
            {
                value = temp;
                return true;
            }
        }
        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given attribute carries a 'ReturnType' setting. If so, returns it and
    /// whether it is a nullable one in the out argument.
    /// </summary>
    public static bool HasReturnType(
        this AttributeData at,
        [NotNullWhen(true)] out INamedTypeSymbol? value, out bool nullable)
    {
        ArgumentNullException.ThrowIfNull(at);
        ArgumentNullException.ThrowIfNull(at.AttributeClass);

        // Generic attribute...
        if (at.AttributeClass.Arity == 1)
        {
            value = (INamedTypeSymbol)at.AttributeClass.TypeArguments[0];
            value = value.UnwrapNullable(out nullable);
            return true;
        }

        // Not-generic attribute...
        else if (at.AttributeClass.Arity == 0)
        {
            if (at.FindNamedArgument(RETURNTYPE, out var arg))
            {
                if (!arg.Value.IsNull && arg.Value.Value is INamedTypeSymbol temp)
                {
                    value = temp.UnwrapNullable(out nullable);
                    return true;
                }
            }
        }

        // Not valid arity, or not found.
        value = null;
        nullable = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the appropriate options to print the given return type, based upon whether it is the
    /// same as the given host one, or not.
    /// </summary>
    public static EasyTypeSymbol ReturnOptions(this INamedTypeSymbol rtype, INamedTypeSymbol host)
    {
        return SymbolEqualityComparer.Default.Equals(host, rtype)
            ? EasyTypeSymbol.Default
            : EasyTypeSymbol.Full with { NullableStyle = IsNullableStyle.None };
    }

    // ====================================================

    extension<T>(IXNode<T> node) where T : ISymbol
    {
        public string SymbolName => node.Symbol.Name;

        public INamedTypeSymbol SymbolType => node.Symbol is IPropertySymbol prop
            ? (INamedTypeSymbol)prop.Type
            : (INamedTypeSymbol)((IFieldSymbol)node.Symbol).Type;

        public string MethodName => $"With{node.SymbolName}";

        // ------------------------------------------------

        /// <summary>
        /// Gets the version of this generator for documentation purposes.
        /// </summary>
        public string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.To3String();

        /// <summary>
        /// Obtains a string with the 'GeneratedCode' attribute for documentation purposes.
        /// </summary>
        public string AttributeDoc => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{node.VersionDoc}}")]
        """;

        /// <summary>
        /// Emits documentation.
        /// </summary>
        public void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// Emulates the '<see langword="with"/>' keyword for the '{{node.SymbolName}}' member.
        /// </summary>
        {{node.AttributeDoc}}
        """);

        // ------------------------------------------------

        /// <summary>
        /// Tries to find a valid method in either the given type, if it is not null, or in any
        /// type in the given chains, in that order. If found, returns it in the out argument.
        /// </summary>
        public bool FindMethod(
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
            [NotNullWhen(true)] out IMethodSymbol? value)
        {
            return Finder.Find(type, chains, out value, (type, out value) =>
            {
                var name = node.SymbolName;
                var argtype = node.SymbolType;

                value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                    x.Name == name &&
                    x.Parameters.Length == 1 &&
                    argtype.IsAssignableTo(x.Parameters[0].Type));

                return value != null;
            });
        }

        // ------------------------------------------------

        /// <summary>
        /// Tries to find a decorated member in either the given type, if it is not null, or in any
        /// type in the given chains, in that order. If found, returns it and its first attribute
        /// found in the out argumenta.
        /// </summary>
        public bool FindMember(
            INamedTypeSymbol? type, IEnumerable<INamedTypeSymbol>[] chains,
            [NotNullWhen(true)] out T? value,
            [NotNullWhen(true)] out AttributeData? at)
        {
            var name = node.SymbolName;

            var found = Finder.Find(type, chains,
                out (T? Member, AttributeData Attribute) info,
                (type, out info) =>
                {
                    var member = type.GetMembers().OfType<T>().FirstOrDefault(x => x.Name == name);
                    if (member != null &&
                        member.HasWithAttribute(out var at))
                    {
                        info = new(member, at);
                        return true;
                    }

                    info = new();
                    return false;
                });

            value = found ? info.Member : default;
            at = found ? info.Attribute : default;
            return found;
        }
    }
}
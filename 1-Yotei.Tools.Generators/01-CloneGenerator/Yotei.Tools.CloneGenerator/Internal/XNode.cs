namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal interface IXNode
{
    public INamedTypeSymbol Symbol { get; }
}

// ========================================================
internal static class XNode
{
    const string USEVIRTUAL = "UseVirtual";
    const string RETURNTYPE = "ReturnType";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is decorated with <see cref="CloneableAttribute"/> or with
    /// <see cref="CloneableAttribute{T}"/> attributes. If so, returns the first one found in the
    /// out argument.
    /// </summary>
    public static bool HasCloneableAttribute(
        this INamedTypeSymbol type,
        [NotNullWhen(true)] out AttributeData? at)
    {
        ArgumentNullException.ThrowIfNull(type);

        var ats = type.GetAttributes([typeof(CloneableAttribute), typeof(CloneableAttribute<>)]).ToArray();
        at = ats.Length > 0 ? ats[0] : null;
        return ats.Length > 0;
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

    extension(IXNode node)
    {
        /// <summary>
        /// Gets the version of this generator for documentation purposes.
        /// </summary>
        public string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.To3String();

        /// <summary>
        /// Obtains a string with the 'GeneratedCode' attribute for documentation purposes.
        /// </summary>
        public string AttributeDoc => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(CloneGenerator)}}", "{{node.VersionDoc}}")]
        """;

        /// <summary>
        /// Emits documentation.
        /// </summary>
        public void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
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
                value = type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
                    x.Name == "Clone" &&
                    x.Parameters.Length == 0 &&
                    x.ReturnsVoid == false);

                return value != null;
            });
        }
    }
}
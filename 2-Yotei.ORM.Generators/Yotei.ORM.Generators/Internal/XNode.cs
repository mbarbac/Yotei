namespace Yotei.ORM.Generators;

// ========================================================
internal interface IXNode
{
    public INamedTypeSymbol Symbol { get; }
}

// ========================================================
internal static class XNode
{
    const string RETURNTYPE = "ReturnType";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is decorated with any 'Invariant'-alike attributes and, if
    /// so, returns them all in the out argument.
    /// </summary>
    public static bool HasInvariantAttributes(
        this ITypeSymbol type,
        out IEnumerable<AttributeData> ats)
    {
        ArgumentNullException.ThrowIfNull(type);

        ats = type.GetAttributes([
            typeof(IInvariantBagAttribute),
            typeof(IInvariantBagAttribute<>),
            typeof(IInvariantListAttribute),
            typeof(IInvariantListAttribute<>),
            typeof(IInvariantListAttribute<,>),
            typeof(InvariantBagAttribute),
            typeof(InvariantBagAttribute<>),
            typeof(InvariantListAttribute),
            typeof(InvariantListAttribute<>),
            typeof(InvariantListAttribute<,>),
        ]);

        return ats.Any();
    }

    /// <summary>
    /// Determines if the given type is decorated with an 'Invariant'-alike attributes and, if
    /// so, returns the first one in the out argument.
    /// </summary>
    public static bool HasInvariantAttribute(
        this ITypeSymbol type,
        [NotNullWhen(true)] out AttributeData? at)
    {
        var found = type.HasInvariantAttributes(out var ats);
        at = found ? ats.First() : null;
        return found;
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

        if (at.FindNamedArgument(RETURNTYPE, out var arg))
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
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantGenerator)}}", "{{node.VersionDoc}}")]
        """;
    }
}
/*

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
 */
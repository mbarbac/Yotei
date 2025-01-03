namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Comparer two <see cref="ISymbol"/> instance for equality, using the options captured. If
/// no comparison can be performed, or if it is not conclusive, then the comparison reverts
/// to the default <see cref="SymbolEqualityComparer"/> one.
/// </summary>
internal record SymbolComparer : IEqualityComparer<ISymbol>
{
    /// <summary>
    /// Initializes a new instance with all settings set to false.
    /// </summary>
    /// <param name="useNamespace"></param>
    /// <param name="useHost"></param>
    /// <param name="useTypeArguments"></param>
    /// <param name="useArguments"></param>
    /// <param name="useNullability"></param>
    /// <param name="useMemberType"></param>
    public SymbolComparer(
        bool useNamespace = false,
        bool useHost = false,
        bool useTypeArguments = false,
        bool useArguments = false,
        bool useNullability = false,
        bool useMemberType = false)
    {
        UseNamespace = useNamespace;
        UseHost = useHost;
        UseTypeArguments = useTypeArguments;
        UseArguments = useArguments;
        UseNullability = useNullability;
        UseMemberType = useMemberType;
    }

    /// <summary>
    /// A shared empty instance with all settings set to false.
    /// </summary>
    public static SymbolComparer Empty { get; } = new();

    /// <summary>
    /// A default shared instance that resembles the <see cref="SymbolEqualityComparer"/> behavior
    /// where nullability and return types are not taken into consideration - the later to allow
    /// covariant return types and methods and/or properties/fields.
    /// </summary>
    public static SymbolComparer Default { get; } = new(
        useNamespace: true,
        useHost: true,
        useTypeArguments: true,
        useArguments: true);

    /// <summary>
    /// A shared instance with all its options set.
    /// </summary>
    public static SymbolComparer Full { get; } = new(
        useNamespace: true,
        useHost: true,
        useTypeArguments: true,
        useArguments: true,
        useNullability: true,
        useMemberType: true);

    // -----------------------------------------------------

    /// <summary>
    /// Use in the comparison the respective namespaces.
    /// </summary>
    public bool UseNamespace { get; init; }

    /// <summary>
    /// Use in the comparison the respective declaring hosts.
    /// </summary>
    public bool UseHost { get; init; }

    /// <summary>
    /// Use in the comparison the respective type arguments.
    /// <br/> This setting is only used with type and method-alike comparisons.
    /// </summary>
    public bool UseTypeArguments { get; init; }

    /// <summary>
    /// Use in the comparison the respective arguments, if any.
    /// <br/> This setting is only used with method and indexer-alike comparisons.
    /// </summary>
    public bool UseArguments { get; init; }

    /// <summary>
    /// Use in the comparison the respective nullability characteristics.
    /// <br/> This setting is only used with type-alike comparisons.
    /// </summary>
    public bool UseNullability { get; init; }

    /// <summary>
    /// Use in the comparison the respective method return type or member type.
    /// <br/> This setting is not used with type-alike comparisons.
    /// </summary>
    public bool UseMemberType { get; init; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(ISymbol? x, ISymbol? y)
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;

        if (x is INamespaceSymbol xns && y is INamespaceSymbol yns) return OnEquals(xns, yns);
        if (x is INamedTypeSymbol xtype && y is INamedTypeSymbol ytype) return OnEquals(xtype, ytype);
        if (x is IMethodSymbol xmethod && y is IMethodSymbol ymethod) return OnEquals(xmethod, ymethod);
        if (x is IPropertySymbol xprop && y is IPropertySymbol yprop) return OnEquals(xprop, yprop);
        if (x is IFieldSymbol xfield && y is IFieldSymbol yfield) return OnEquals(xfield, yfield);

        return UseNullability
            ? SymbolEqualityComparer.IncludeNullability.Equals(x, y)
            : SymbolEqualityComparer.Default.Equals(x, y);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compare the two given symbols.
    /// </summary>
    bool OnEquals(INamespaceSymbol x, INamespaceSymbol y)
    {
        if (UseNamespace)
        {
            var xhost = x.ContainingSymbol;
            var yhost = y.ContainingSymbol;

            if (!Equals(xhost, yhost)) return false;
        }

        return string.Compare(x.Name, y.Name) == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compare the two given symbols.
    /// </summary>
    bool OnEquals(INamedTypeSymbol x, INamedTypeSymbol y)
    {
        var xgen = x.TypeKind == TypeKind.TypeParameter;
        var ygen = y.TypeKind == TypeKind.TypeParameter;
        if (xgen || ygen) return true;

        if (UseNamespace || UseHost)
        {
            var xoptions = this with { UseNullability = false };

            var xhost = x.ContainingSymbol;
            var yhost = y.ContainingSymbol;

            if (!xoptions.Equals(xhost, yhost)) return false;
        }

        if (string.Compare(x.Name, y.Name) != 0) return false;

        if (UseTypeArguments)
        {
            var xargs = x.TypeArguments;
            var yargs = y.TypeArguments;

            if (xargs.Length != yargs.Length) return false;

            for (int i = 0; i < xargs.Length; i++)
            {
                var xarg = xargs[i];
                var yarg = yargs[i];

                if (!Equals(xarg, yarg)) return false;
            }
        }

        if (UseNullability)
        {
            if (x.Name == "Nullable" && y.Name == "Nullable")
            {
                if (x.Arity == 1 && y.Arity == 1)
                    return Equals(x.TypeArguments[0], y.TypeArguments[0]);
            }
            if (x.Name == "Nullable")
            {
                if (x.Arity == 1) return Equals(x.TypeArguments[0], y);
            }
            if (y.Name == "Nullable")
            {
                if (y.Arity == 1) return Equals(x, y.TypeArguments[0]);
            }

            var xnull = x.NullableAnnotation == NullableAnnotation.Annotated;
            var ynull = y.NullableAnnotation == NullableAnnotation.Annotated;
            if (xnull != ynull) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compare the two given symbols.
    /// </summary>
    bool OnEquals(IMethodSymbol x, IMethodSymbol y)
    {
        if (UseMemberType)
        {
            var xtype = x.ReturnType;
            var ytype = y.ReturnType;

            if (!Equals(xtype, ytype)) return false;
        }

        if (UseNamespace || UseHost)
        {
            var xoptions = this with { UseNullability = false };

            var xhost = x.ContainingType;
            var yhost = y.ContainingType;

            if (!xoptions.Equals(xhost, yhost)) return false;
        }

        if (string.Compare(x.Name, y.Name) != 0) return false;

        if (UseTypeArguments)
        {
            var xargs = x.TypeArguments;
            var yargs = y.TypeArguments;

            if (xargs.Length != yargs.Length) return false;

            for (int i = 0; i < xargs.Length; i++)
            {
                var xarg = xargs[i];
                var yarg = yargs[i];

                if (!Equals(xarg, yarg)) return false;
            }
        }

        if (UseArguments)
        {
            var xpars = x.Parameters;
            var ypars = y.Parameters;

            if (xpars.Length != ypars.Length) return false;

            for (int i = 0; i < xpars.Length; i++)
            {
                var xpar = xpars[i];
                var ypar = ypars[i];

                if (!Equals(xpar.Type, ypar.Type)) return false;
            }
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compare the two given symbols.
    /// </summary>
    bool OnEquals(IPropertySymbol x, IPropertySymbol y)
    {
        if (UseMemberType)
        {
            var xtype = x.Type;
            var ytype = y.Type;

            if (!Equals(xtype, ytype)) return false;
        }

        if (UseNamespace || UseHost)
        {
            var xoptions = this with { UseNullability = false };

            var xhost = x.ContainingType;
            var yhost = y.ContainingType;

            if (!xoptions.Equals(xhost, yhost)) return false;
        }

        if (string.Compare(x.Name, y.Name) != 0) return false;

        if (UseArguments)
        {
            var xpars = x.Parameters;
            var ypars = y.Parameters;

            if (xpars.Length != ypars.Length) return false;

            for (int i = 0; i < xpars.Length; i++)
            {
                var xpar = xpars[i];
                var ypar = ypars[i];

                if (!Equals(xpar.Type, ypar.Type)) return false;
            }
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to compare the two given symbols.
    /// </summary>
    bool OnEquals(IFieldSymbol x, IFieldSymbol y)
    {
        if (UseMemberType)
        {
            var xtype = x.Type;
            var ytype = y.Type;

            if (!Equals(xtype, ytype)) return false;
        }

        if (UseNamespace || UseHost)
        {
            var xoptions = this with { UseNullability = false };

            var xhost = x.ContainingType;
            var yhost = y.ContainingType;

            if (!xoptions.Equals(xhost, yhost)) return false;
        }

        if (string.Compare(x.Name, y.Name) != 0) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int GetHashCode(ISymbol? obj) => SymbolEqualityComparer.Default.GetHashCode(obj);
}
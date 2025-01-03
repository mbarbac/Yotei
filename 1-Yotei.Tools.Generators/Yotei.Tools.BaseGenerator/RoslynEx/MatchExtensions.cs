namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class MatchExtensions
{
    /// <summary>
    /// Determines if the given type symbol matches the given regular type. or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool Match(this ITypeSymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        // Obvious things...
        if (symbol.IsNamespace) return false; // Symbols may be namespaces...
        if (symbol.Kind == SymbolKind.TypeParameter) return true; // Parameters always match...
        if (type.IsGenericParameter) return true; // Generic regular parameters always match...

        // Initializing...
        var shost = symbol.ContainingType;
        var thost = type.DeclaringType;

        var sargs = (symbol as INamedTypeSymbol)?.TypeArguments
            ?? ImmutableArray<ITypeSymbol>.Empty;

        var targs = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is System.Reflection.TypeInfo info ? info.GenericTypeParameters : [];

        if (sargs.Length != targs.Length) return false; // Shortcut...

        // Namespaces...
        if (shost == null && thost == null)
        {
            var sspace = symbol.ContainingNamespace?.ToString() ?? string.Empty;
            var tspace = type.Namespace ?? string.Empty;

            if (sspace != tspace) return false;
        }
        else if (shost == null || thost == null) return false;

        // Nested types...
        if (shost != null && thost != null)
        {
            if (!shost.Match(thost)) return false;
        }
        else if (shost != null || thost != null) return false;

        // Names...
        var sname = symbol.Name; if (sargs.Length > 0) sname += $"`{sargs.Length}";
        var tname = type.Name;
        if (sname != tname) return false;

        // Type Arguments...
        for (int i = 0; i < sargs.Length; i++)
        {
            var sarg = sargs[i];
            var targ = targs[i];

            if (!sarg.Match(targ)) return false;
        }

        // Finishing...
        return true;
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type symbol of the class of the given attribute matches the given
    /// regular type, or not.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool Match(this AttributeData attribute, Type type)
    {
        attribute.ThrowWhenNull();
        type.ThrowWhenNull();

        if (attribute.AttributeClass != null) return attribute.AttributeClass.Match(type);
        return false;
    }

    // ---------------------------------------------------

    /// <summary>
    /// Extracts from the given collection the <see cref="AttributeData"/> elements whose class
    /// type symbols match the given regular type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> SelectMatch(
        this IEnumerable<AttributeData> source, Type type)
    {
        source.ThrowWhenNull();
        type.ThrowWhenNull();

        return source.Where(x => x.Match(type));
    }
}
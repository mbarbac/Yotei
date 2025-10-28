namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class MatchExtensions
{
    /// <summary>
    /// Determines if the given type symbol conceptually matches the given regular type, or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// NOTE: Currently, generic type symbols or generic regular types always match.
    /// NOTE: Currently, we don't take into consideration restrictions applied the elements.
    public static bool Match(this ITypeSymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        // Trivial cases...
        if (symbol.IsNamespace) return false;
        if (symbol.Kind == SymbolKind.TypeParameter) return true;
        if (type.IsGenericParameter) return true;

        // Capturing type arguments...
        var s_args = (symbol as INamedTypeSymbol)?.TypeArguments ?? [];
        var t_args = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is System.Reflection.TypeInfo info ? info.GenericTypeParameters : [];

        if (s_args.Length != t_args.Length) return false; // Shortcut...

        // Namespaces...
        var s_host = symbol.ContainingType;
        var t_host = type.DeclaringType;

        if (s_host == null && t_host == null)
        {
            var s_space = symbol.ContainingNamespace?.ToString() ?? string.Empty;
            var t_space = type.Namespace ?? string.Empty;

            if (s_space != t_space) return false;
        }
        else if (s_host == null || t_host == null) return false;

        // Nested types...
        if (s_host != null && t_host != null)
        {
            if (!s_host.Match(t_host)) return false;
        }
        else if (s_host != null || t_host != null) return false;

        // Names...
        var s_name = symbol.Name; if (s_args.Length > 0) s_name += $"`{s_args.Length}";
        var t_name = type.Name;
        if (s_name != t_name) return false;

        // Processing type arguments...
        for (int i = 0; i < s_args.Length; i++)
        {
            var sarg = s_args[i];
            var targ = t_args[i];

            if (!sarg.Match(targ)) return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type symbol of the class specified by the given attribute data matches
    /// the given regular type, or not.
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

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given collection of attribute data the ones whose classes match the
    /// given regular type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> SelectMatch(
        this IEnumerable<AttributeData> source,
        Type type)
    {
        source.ThrowWhenNull();
        type.ThrowWhenNull();

        return source.Where(x => x.Match(type));
    }
}
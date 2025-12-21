namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class MatchExtensions
{
    /// <summary>
    /// Determines if the given type symbol matches any of the given regular types.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static bool MatchAny(this ITypeSymbol symbol, Type[] types)
    {
        symbol.ThrowWhenNull();
        types.ThrowWhenNull();

        return types.Any(x => symbol.Match(x));
    }

    /// <summary>
    /// Determines if the given type symbol matches the given regular type.
    /// </summary>
    /// LOW: Match(symbol, type): there might be constrains in symbol or type we should check when both represent generic types.
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool Match(this ITypeSymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        // Trivial cases...
        if (symbol.IsNamespace) return false;
        if (symbol.Kind == SymbolKind.TypeParameter) return true;
        if (type.IsGenericParameter) return true;

        // Capturing...
        var sargs = (symbol as INamedTypeSymbol)?.TypeArguments ?? [];
        var targs = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is System.Reflection.TypeInfo info ? info.GenericTypeParameters : [];

        if (sargs.Length != targs.Length) return false; // shortcut...

        // Names...
        var sname = symbol.Name;
        var tname = type.Name;
        var index = tname.IndexOf('`');
        if (index >= 0) tname = tname[..index];
        if (sname != tname) return false;

        // Hierarchy...
        var shost = symbol.ContainingType;
        var thost = type.DeclaringType;

        if (shost is null && thost is null) // Namespaces...
        {
            var sspace = symbol.ContainingNamespace?.ToString() ?? string.Empty;
            var tspace = type.Namespace ?? string.Empty;
            if (sspace != tspace) return false;
        }
        else if (shost is null || thost is null) return false;

        if (shost is not null && thost is not null) // Nested types...
        {
            if (!shost.Match(thost)) return false;
        }
        else if (shost is not null || thost is not null) return false;

        // Type arguments...
        var count = sargs.Length;
        for (int i = 0; i < count; i++)
        {
            var sarg = sargs[i];
            var targ = targs[i];
            if (!sarg.Match(targ)) return false;
        }

        // Finishing...
        return true;
    }
}
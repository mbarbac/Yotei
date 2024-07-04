using SystemTypeInfo = System.Reflection.TypeInfo;

namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class MatchExtensions
{
    /// <summary>
    /// Determines if the given attribute matches the given type.
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

    /// <summary>
    /// Determines if the given symbol matches the given type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool Match(this ITypeSymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        // Intercepting symbols that are not types...
        if (symbol.IsNamespace) return false;

        // Intercepting generics...
        if (symbol.Kind == SymbolKind.TypeParameter) return true;
        if (type.IsGenericParameter) return true;

        // Capturing variables...
        var shost = symbol.ContainingType;
        var thost = type.DeclaringType;

        var sargs = (symbol as INamedTypeSymbol)?.TypeArguments
            ?? ImmutableArray<ITypeSymbol>.Empty;
        
        var targs = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is SystemTypeInfo info ? info.GenericTypeParameters : [];

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
}
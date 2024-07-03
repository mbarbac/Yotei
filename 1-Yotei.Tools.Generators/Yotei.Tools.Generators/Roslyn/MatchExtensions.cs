#pragma warning disable IDE0019

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
    public static bool Match(this INamedTypeSymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        // Names...
        var sname = symbol.Name;        
        var tname = type.Name;
        var index = tname.IndexOf('`'); if (index > 0) tname = tname[..index];

        if (sname != tname) return false;

        // Type Arguments...
        var sargs = symbol.TypeArguments;
        var targs = type.GenericTypeArguments.Length != 0
            ? type.GenericTypeArguments
            : type is SystemTypeInfo info ? info.GenericTypeParameters : [];

        if (sargs.Length != targs.Length) return false;
        for (int i = 0; i < sargs.Length; i++)
        {
            var sarg = sargs[i] as INamedTypeSymbol; if (sarg == null) return false;
            var targ = targs[i];

            var sgen = sarg.TypeKind == TypeKind.Error;
            var tgen = targ.IsGenericParameter;
            if (sgen || tgen) continue;

            if (!sarg.Match(targ)) return false;
        }

        // Finishing...
        return true;
    }
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class TypeExtensions
{
    /// <summary>
    /// Determines if the given type is an interface or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsInterface(
        this ITypeSymbol type) => type.TypeKind == TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// Enumerates, in inheritance order, the base types of the given one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<ITypeSymbol> AllBaseTypes(this ITypeSymbol type)
    {
        while (true)
        {
            type = type.BaseType!;

            if (type != null && !type.IsNamespace) yield return type;
            else break;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if an instance of the given source type can be assigned to the given target
    /// one. This method validates both the base types of the source type, if any, as well as
    /// its interfaces.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsAssignableTo(this ITypeSymbol source, ITypeSymbol target)
    {
        // Same type...
        var comparer = SymbolEqualityComparer.Default;
        if (comparer.Equals(source, target)) return true;

        // Parent elements...
        if (source.AllBaseTypes().Any(x => comparer.Equals(target, x))) return true;
        if (source.AllInterfaces.Any(x => comparer.Equals(target, x))) return true;

        // Not found...
        return false;
    }
}
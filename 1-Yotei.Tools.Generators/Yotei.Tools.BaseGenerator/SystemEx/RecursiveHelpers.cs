namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class RecursiveHelper
{
    /// <summary>
    /// The delegate to invoke to obtain a requested value.
    /// </summary>
    public delegate T? Finder<T>(INamedTypeSymbol type, out bool found);

    /// <summary>
    /// Tries to obtain a requested value using the given delegate with the given type, or with
    /// the types in the given collections (ie: its base types or interfaces).
    /// </summary>
    public static T? Recursive<T>(
        this INamedTypeSymbol type, Finder<T> func,
        out bool found,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        func.ThrowWhenNull();
        var value = func(type, out found);
        if (found) return value;

        chains.ThrowWhenNull();
        foreach (var items in chains)
        {
            items.ThrowWhenNull();
            foreach (var item in items)
            {
                item.ThrowWhenNull();
                value = func(item, out found);
                if (found) return value;
            }
        }

        return default;
    }
}
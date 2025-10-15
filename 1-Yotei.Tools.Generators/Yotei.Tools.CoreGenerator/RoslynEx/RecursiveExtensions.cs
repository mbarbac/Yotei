namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides methods to find the first type among the ones in several collections.
/// </summary>
internal static class Finder
{
    /// <summary>
    /// Returns the first type element, among the given collections, that matches the given
    /// predicate. Returns '<c>null</c>' if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? First(
        Predicate<INamedTypeSymbol> predicate,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        predicate.ThrowWhenNull();
        chains.ThrowWhenNull();

        foreach (var chain in chains)
        {
            chain.ThrowWhenNull();
            foreach (var item in chain)
            {
                item.ThrowWhenNull();
                if (predicate(item)) return item;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the first type element, among the given head one and the given collections, that
    /// matches the given predicate. Returns '<c>null</c>' if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? First(
        Predicate<INamedTypeSymbol> predicate,
        INamedTypeSymbol head,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        predicate.ThrowWhenNull();
        head.ThrowWhenNull();
        chains.ThrowWhenNull();

        if (predicate(head)) return head;

        foreach (var chain in chains)
        {
            chain.ThrowWhenNull();
            foreach (var item in chain)
            {
                item.ThrowWhenNull();
                if (predicate(item)) return item;
            }
        }
        return null;
    }
}
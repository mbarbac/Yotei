namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class RecursiveFinders
{
    /// <summary>
    /// The signature of the delegate to invoke to find a value of the requested type using the
    /// given symbol. If found, it shall return 'true' and the value itself in the out parameter,
    /// or return 'false' otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool FinderDelegate<T>(INamedTypeSymbol type, out T value);

    /// <summary>
    /// Tries to find a value of the requested type using the given predicate fed with the given
    /// type elements. If requested, the host itself is the first one tested. Otherwise, the ones
    /// from the given chains are tested in order. The first match amomg all these elements is the
    /// one returned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="useHost"></param>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool Finder<T>(
        this INamedTypeSymbol type,
        bool useHost,
        FinderDelegate<T> predicate,
        out T value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        predicate.ThrowWhenNull();
        chains.ThrowWhenNull();

        if (useHost && predicate(type, out value)) return true;

        foreach (var chain in chains)
        {
            chain.ThrowWhenNull();
            foreach (var child in chain)
            {
                child.ThrowWhenNull();
                if (predicate(child, out value)) return true;
            }
        }

        value = default!;
        return false;
    }
}
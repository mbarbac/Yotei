namespace Yotei.Tools.TreeGenerator;

// ========================================================
/// <summary>
/// Provides recursive tree-oriented find capabilities to obtain an arbitrary value from a given
/// collection of types.
/// </summary>
internal static class Finder
{
    /// <summary>
    /// The delegate that tries to obtain the requested value from the given type. Returns true
    /// if the value was obtained, or false otherwise. In the later case, the value of the out
    /// argument is undefined and should be ignored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool FindDelegate<T>(INamedTypeSymbol type, out T value);

    /// <summary>
    /// Tries to obtain the requested value from the given given collection of chains (each being
    /// a collection of types), in order, where the first match invoking the given predicate wins.
    /// If so, returns true and the found value in the out argument. Otherwise, returns false and
    /// the value of the out argument is undefined and should be ignored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="chains"></param>
    /// <param name="value"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Find<T>(
        IEnumerable<INamedTypeSymbol>[] chains,
        out T value,
        FindDelegate<T> predicate)
        => Find<T>(null, chains, out value, predicate);

    /// <summary>
    /// Tries to obtain the requested value from the given host (if not null), and from the given
    /// collection of chains (each being a collection of types), in that order, where the first
    /// match invoking the given predicate wins. If so, returns true and the found value in the
    /// out argument. Otherwise, returns false and the value of the out argument is undefined and
    /// should be ignored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="host"></param>
    /// <param name="chains"></param>
    /// <param name="value"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Find<T>(
        INamedTypeSymbol? host, IEnumerable<INamedTypeSymbol>[] chains,
        out T value,
        FindDelegate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(chains);
        ArgumentNullException.ThrowIfNull(predicate);

        // The given type, if not null...
        if (host != null && predicate(host, out value)) return true;

        // The types of the given arrays, if any...
        foreach (var chain in chains)
        {
            ArgumentNullException.ThrowIfNull(chain);
            foreach (var item in chain)
            {
                ArgumentNullException.ThrowIfNull(item, "type");
                if (predicate(item, out value)) return true;
            }
        }

        value = default!;
        return false;
    }
}
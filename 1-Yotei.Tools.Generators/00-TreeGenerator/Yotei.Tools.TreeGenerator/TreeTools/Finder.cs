namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Provides recursive tree-oriented find capabilities to obtain an arbitrary value from a given
/// type and collection of types.
/// </summary>
public static class Finder
{
    /// <summary>
    /// The delegate invoked to obtain the requested value from the given type. Returns true if
    /// such is possible (and sets the value of the out argument), or false otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool FindDelegate<T>(INamedTypeSymbol type, out T value);

    /// <summary>
    /// Tries to obtain the requested value from the first type in the given collection of chains,
    /// each carrying its own types, that match the given predicate, in order. If found, returns
    /// true (and sets the value of the out argument). Otherwise, returns false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="chains"></param>
    /// <param name="value"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Find<T>(
        IEnumerable<INamedTypeSymbol>[] chains,
        out T value,
        FindDelegate<T> predicate) => Find(null, chains, out value, predicate);

    /// <summary>
    /// Tries to obtain the requested value from either the given host type, if not null, or from
    /// the first type in the given collection of chains, each carrying its own types, that match
    /// the given predicate, in order. If found, returns true (and sets the value of the out
    /// argument). Otherwise, returns false.
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

        // Trying the host type...
        if (host is not null && predicate(host, out value)) return true;

        // Trying the types in the given chains...
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
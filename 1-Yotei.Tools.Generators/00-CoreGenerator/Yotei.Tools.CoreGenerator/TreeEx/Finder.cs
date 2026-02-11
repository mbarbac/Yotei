namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Provides recursive find capabilities to obtain a value from a given collection or collections
/// of types, using given predicates.
/// </summary>
internal static class Finder
{
    /// <summary>
    /// The signature of the delegate invoked to obtain a value from a give type. If found, it
    /// shall return <see langword="true"/> and the found value in the out argument. If not, it
    /// returns <see langword="false"/> and set the out value to an arbitrary one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool FindDelegate<T>(INamedTypeSymbol type, out T value);

    /// <summary>
    /// Tries to obtain a value from the given type, and if not from the types in the given set of
    /// types' arrays, in this order, using the given finder predicate. If found, the value is set
    /// on the out argument and the method returns <see langword="true"/>. Otherwise, returns
    /// <see langword="false"/> and sets the out argument to an arbitrary value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool Find<T>(
        FindDelegate<T> predicate,
        out T value,
        INamedTypeSymbol type,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(chains);

        if (predicate(type, out value)) return true;
        return Find(predicate, out value, chains);
    }

    /// <summary>
    /// Tries to obtain a value from the types in the given set of types' arrays, in this order,
    /// using the given finder predicate. If found, the value is set on the out argument and the
    /// method returns <see langword="true"/>. Otherwise, returns <see langword="false"/> and sets
    /// the out argument to an arbitrary value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool Find<T>(
        FindDelegate<T> predicate,
        out T value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(chains);

        foreach (var chain in chains)
        {
            ArgumentNullException.ThrowIfNull(chain);
            foreach (var type in chain)
            {
                ArgumentNullException.ThrowIfNull(type);
                if (predicate(type, out value)) return true;
            }
        }

        value = default!;
        return false;
    }
}
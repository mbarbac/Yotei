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
    /// returns <see langword="false"/> and sets the out value to an arbitrary one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool FindDelegate<T>(INamedTypeSymbol type, out T value);

    /// <summary>
    /// Tries to obtain a <typeparamref name="T"/> value using the given predicate on the given
    /// type, if it is not null, and on the types of the given chains, in that order. If found,
    /// returns the value in the out argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <param name="value"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Find<T>(
        INamedTypeSymbol? type,
        IEnumerable<INamedTypeSymbol>[] chains,
        out T value,
        FindDelegate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(chains);

        // The given type, if not null...
        if (type != null && predicate(type, out value)) return true;

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
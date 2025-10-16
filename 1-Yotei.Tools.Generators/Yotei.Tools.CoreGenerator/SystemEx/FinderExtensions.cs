namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class FinderHelpers
{
    /// <summary>
    /// The signature of the delegate to invoke to obtain a value of the requested type using
    /// the given symbol. If found, these delegates shall return 'true' and the value itself in
    /// the out parameter, or return 'false' otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool Finder<T>(INamedTypeSymbol type, out T value);

    static void Example()
    {
        INamedTypeSymbol type = null;
    }

    /// <summary>
    /// Tries to find a value of the requested type using the given finder predicate with the
    /// given chains, qualifying the first match in order. If found, returns 'true' and sets the
    /// out argument. Otherwise, returns 'false'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool Find<T>(
        this Finder<T> predicate, out T value, params INamedTypeSymbol[] chains)
    {
        throw null;
    }

    /// <summary>
    /// Tries to find a value of the requested type using the given finder predicate with the
    /// given symbol and the given symbol chains, qualifying the first match in that order. If
    /// found, returns 'true' and sets the out argument. Otherwise, returns 'false'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool Find<T>(
        this Finder<T> predicate, out T value, INamedTypeSymbol type, params INamedTypeSymbol[] chains)
    {
        predicate.ThrowWhenNull();
        type.ThrowWhenNull();
        chains.ThrowWhenNull();

        if (predicate(type, out value)) return true;
        return predicate.Find(out value, chains);
    }
}
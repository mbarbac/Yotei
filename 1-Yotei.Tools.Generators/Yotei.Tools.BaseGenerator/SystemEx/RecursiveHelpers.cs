namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class RecursiveHelper
{
    /// <summary>
    /// The signature of the delegate to invoke to obtain the requested value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool Finder<T>(INamedTypeSymbol type, out T value);

    /// <summary>
    /// Tries to obtain a requested value using the given delegate on the given type, or if not
    /// found, with the types in the optional collections. Returns whether the value has been
    /// found or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="func"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool Recursive<T>(
        this INamedTypeSymbol type,
        Finder<T> func,
        out T value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        func.ThrowWhenNull();
        chains.ThrowWhenNull();

        var found = func(type, out value);
        if (found) return true;

        foreach (var chain in chains)
        {
            chain.ThrowWhenNull();
            foreach (var item in chain)
            {
                found = func(item, out value);
                if (found) return true;
            }
        }

        return false;
    }
}
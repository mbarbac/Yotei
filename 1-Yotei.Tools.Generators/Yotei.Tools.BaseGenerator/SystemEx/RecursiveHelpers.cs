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

        var found = func(type, out value);
        if (found) return true;

        return type.RecursiveOnly(func, out value, chains);
    }

    /// <summary>
    /// Tries to obtain a requested value using the given delegate on the types in the optional
    /// given chains, but not on the given one itself. Returns whether the value has been found
    /// or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="func"></param>
    /// <param name="value"></param>
    /// <param name="chains"></param>
    /// <returns></returns>
    public static bool RecursiveOnly<T>(
        this INamedTypeSymbol type,
        Finder<T> func,
        out T value,
        params IEnumerable<INamedTypeSymbol>[] chains)
    {
        type.ThrowWhenNull();
        func.ThrowWhenNull();
        chains.ThrowWhenNull();

        foreach (var chain in chains)
        {
            chain.ThrowWhenNull();
            foreach (var item in chain)
            {
                var found = func(item, out value);
                if (found) return true;
            }
        }

        value = default!;
        return false;
    }
}
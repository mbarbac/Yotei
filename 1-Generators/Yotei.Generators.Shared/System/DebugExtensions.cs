namespace Yotei.Generators;

// ========================================================
internal static class DebugExtensions
{
#if DEBUG
    // ----------------------------------------------------
    /// <summary>
    /// Returns an array from the given range of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static T[] ToDebugArray<T>(this IEnumerable<T> items) => items.ToArray();
#else
    // ----------------------------------------------------
    /// <summary>
    /// Returns the enumerable object itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static IEnumerable<T> ToDebugArray<T>(this IEnumerable<T> items) => items;
#endif
}
namespace Yotei.Tools;

// ========================================================
public static class DebugExtensions
{
    /// <summary>
    /// Returns either an array, in debug mode, or the original enumerable object otherwise.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
#if DEBUG
    public static T[] ToDebugArray<T>(this IEnumerable<T> items) => [.. items];
#else
    public static IEnumerable<T> ToDebugArray<T>(this IEnumerable<T> items) => items;
#endif
}
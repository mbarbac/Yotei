namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class DebugExtensions
{
#if DEBUG_
    public static T[] ToDebugArray<T>(this IEnumerable<T> items) => items.ToArray();
#else
    public static IEnumerable<T> ToDebugArray<T>(this IEnumerable<T> items) => items;
#endif
}
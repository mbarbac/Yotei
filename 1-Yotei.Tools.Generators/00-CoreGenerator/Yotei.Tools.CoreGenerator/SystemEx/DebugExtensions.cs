namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class DebugExtensions
{
#if DEBUG
    public static T[] ToDebugArray<T>(this IEnumerable<T> items) => [.. items];
#else
    public static IEnumerable<T> ToDebugArray<T>(this IEnumerable<T> items) => items;
#endif
}
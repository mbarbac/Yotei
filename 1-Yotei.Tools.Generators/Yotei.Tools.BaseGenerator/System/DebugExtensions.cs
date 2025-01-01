namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class DebugExtensions
{
#if DEBUG
    public static T[] ToDebugArray<T>(this IEnumerable<T> items) => items.ToArray();
#else
    public static IEnumerable<T> ToDebugArray<T>(this IEnumerable<T> items) => items;
#endif
}
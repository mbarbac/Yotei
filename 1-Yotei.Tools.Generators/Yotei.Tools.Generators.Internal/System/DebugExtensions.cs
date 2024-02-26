namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class DebugExtensions
{
#if DEBUG
    public static T[] ToDebugArray<T>(this IEnumerable<T> source) => source.ToArray();
#else
    public static IEnumerable<T> ToDebugArray<T>(this IEnumerable<T> source) => source;
#endif
}
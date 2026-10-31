#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static class DebugExtensions
{
    /// <summary>
    /// Returns either an array using the elements of the given range, if this project was
    /// compiled in debug mode, or the original range otherwise.
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
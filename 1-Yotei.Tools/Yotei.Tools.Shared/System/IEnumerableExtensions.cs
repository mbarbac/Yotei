#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
static class IEnumerableExtensions
{
    /// <summary>
    /// Executes the given action for each element in the given enumeration.
    /// </summary>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in items) action(item);
    }

    /// <summary>
    /// Executes the given action for each element in the given enumeration that matches the
    /// given predicate.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(
        this IEnumerable<T> items, Predicate<T> predicate, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in items) if (predicate(item)) action(item);
    }
}
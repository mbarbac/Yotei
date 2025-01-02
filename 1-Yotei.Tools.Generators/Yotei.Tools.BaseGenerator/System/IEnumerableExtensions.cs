namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class IEnumerableExtensions
{
    /// <summary>
    /// Executes the given action for each of the elements in the enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        items.ThrowWhenNull();
        action.ThrowWhenNull();

        foreach (T item in items) action(item);
    }

    /// <summary>
    /// Executes the given action for each of the elements in the enumeration that match the
    /// given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="predicate"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(
        this IEnumerable<T> items, Predicate<T> predicate, Action<T> action)
    {
        items.ThrowWhenNull();
        predicate.ThrowWhenNull();
        action.ThrowWhenNull();

        foreach (T item in items) if (predicate(item)) action(item);
    }
}
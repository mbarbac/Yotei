#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class ReadOnlySpanExtensions
{
#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the first index of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    //public static int IndexOfAny<T>(this ReadOnlySpan<T> source, IEnumerable<T> values)
    //{
    //    var comparer = new InnerComparer<T>();
    //    return source.IndexOfAny(values, comparer);
    //}
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the first index of any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(item, value)) return i;
        }
        return -1;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the last index of any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(item, value)) return i;
        }
        return -1;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span contains any of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values)
    {
        var comparer = new EqualityComparer<T>();
        return source.IndexOfAny(values, comparer) >= 0;
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span contains any of any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;
#endif
}
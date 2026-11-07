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
    /// Searches for the first index of any of the given values, using the given element's
    /// comparer.
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
    /// Searches for the last index of any of the given values, using the given elements' comparer.
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
    /// Determines if the span contains any of any of the given values, using the given elements'
    /// comparer.
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

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines the order of the span relative to the sequence of values, using the given
    /// elements' comparer. Returns:
    /// <br/>- Less than 0: span precedes the values' sequence.
    /// <br/>- Equal to 0: span equals the values' sequence.
    /// <br/>- Greater than 0: span follows the values' sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int SequenceCompareTo<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        using var iter = values.GetEnumerator();
        for (int i = 0; i < source.Length; i++)
        {
            if (!iter.MoveNext()) return +1;

            var xvalue = source[i];
            var yvalue = iter.Current;
            var result = comparer.Compare(xvalue, yvalue); if (result != 0) return result;
        }
        if (iter.MoveNext()) return -1;
        return 0;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span shall be considered equal to the values' sequence, using the given
    /// elements' comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        using var iter = values.GetEnumerator();
        for (int i = 0; i < source.Length; i++)
        {
            if (!iter.MoveNext()) return false;

            var xvalue = source[i];
            var yvalue = iter.Current;
            var result = comparer.Equals(xvalue, yvalue); if (!result) return false;
        }
        if (iter.MoveNext()) return false;
        return true;
    }
#endif
}
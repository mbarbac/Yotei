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
    /// <summary>
    /// Determines the order of this span relative to the given sequence of values:
    /// <br/>- Less than 0: span precedes the values' sequence.
    /// <br/>- Equal to 0: span equals the values' sequence.
    /// <br/>- Greater than 0: span follows the values' sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public static int SequenceCompareTo<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IComparable<T>
    {
        var comparer = ComparerEx.New<T>();
        return source.SequenceCompareTo(values, comparer);
    }

    /// <summary>
    /// Determines the order of this span relative to the given sequence of values:
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

        var len = source.Length;
        var iter = values.GetEnumerator();

        for (int i = 0; i < len; i++)
        {
            if (!iter.MoveNext()) return +1;

            var xvalue = source[i];
            var yvalue = iter.Current;
            var result = comparer.Compare(xvalue, yvalue);
            if (result != 0) return result < 0 ? -1 : +1;
        }
        if (iter.MoveNext()) return -1;
        return 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines is the contents of the span can be considered the same as the other given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.SequenceEqual(values, comparer);
    }

    /// <summary>
    /// Determines is the contents of the span can be considered the same as the other given ones.
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

        var len = source.Length;
        var iter = values.GetEnumerator();

        for (int i = 0; i < len; i++)
        {
            if (!iter.MoveNext()) return false;

            var xvalue = source[i];
            var yvalue = iter.Current;
            var result = comparer.Equals(xvalue, yvalue);
            if (!result) return false;
        }
        if (iter.MoveNext()) return false;
        return true;
    }
}
/*  SequenceEqual
 *  
 *  IndexOf
 *  LastIndexOf
 *  IndexesOf
 *  Contains
 *  StartsWith
 *  EndsWith
 *  
 *  TrimStart
 *  TrimEnd
 *  Trim
 *  
 *  IndexOfAny
 *  LastIndexOfAny
 *  IndexesOfAny
 *  ContainsAny
 *  StartsWithAny
 *  EndsWithAny
 *  
 *  IndexOfAnyExcept
 *  LastIndexOfAnyExcept
 *  IndexesOfAnyExcept
 *  ContainsAnyExcept
 */
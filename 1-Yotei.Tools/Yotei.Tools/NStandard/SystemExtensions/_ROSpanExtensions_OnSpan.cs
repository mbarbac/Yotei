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
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int SequenceCompareTo<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var xlen = source.Length;
        var ylen = values.Length;

        if (xlen < ylen) return -1;
        if (xlen > ylen) return +1;

        for (int i = 0; i < xlen; i++)
        {
            var xvalue = source[i];
            var yvalue = values[i];
            var result = comparer.Compare(xvalue, yvalue);
            if (result != 0) return result < 0 ? -1 : +1;
        }
        return 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines is the contents of the span can be considered the same as the other given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var xlen = source.Length;
        var ylen = values.Length;

        if (xlen < ylen) return false;
        if (xlen > ylen) return false;

        for (int i = 0; i < xlen; i++)
        {
            var xvalue = source[i];
            var yvalue = values[i];
            var result = comparer.Equals(xvalue, yvalue);
            if (!result) return false;
        }
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
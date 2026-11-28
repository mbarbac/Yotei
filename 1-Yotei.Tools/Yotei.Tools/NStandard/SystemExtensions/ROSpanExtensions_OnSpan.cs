#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class ReadOnlySpanExtensions
{
#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the contents of this array are the same as ones in the given sequence.
    /// </summary>
    /// <remarks>
    /// To mimic standard behavior, no 'where T : IEquatable{T}' restriction is imposed. This
    /// is seems to be very specific for arrays, other collection do have this restriction.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.SequenceEqual(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the contents of this array are the same as ones in the given sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        var xlen = source.Length;
        var ylen = values.Length;
        if (xlen != ylen) return false;

        for (int i = 0; i < xlen; i++)
        {
            var xvalue = source[i];
            var yvalue = values[i];
            var same = comparer.Equals(xvalue, yvalue);
            if (!same) return false;
        }
        return true;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
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
        ArgumentNullException.ThrowIfNull(comparer);

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
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first ocurrence of the given sequence, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        if (source.Length == 0 && values.Length == 0) return 0; // Standard behavior!
        if (source.Length == 0) return -1;
        if (values.Length == 0) return -1;
        if (values.Length > source.Length) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            var match = 0; for (int k = 0; k < values.Length; k++)
            {
                var j = i + k; if (j >= source.Length) break;
                var x = source[j];
                var y = values[k];
                if (comparer.Equals(x, y)) match++; else break;
            }
            if (match == values.Length) return i;
        }
        return -1;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last ocurrence of the given sequence, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        if (source.Length == 0 && values.Length == 0) return 0; // Standard behavior!
        if (source.Length == 0) return -1;
        if (values.Length == 0) return -1;
        if (values.Length > source.Length) return -1;

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var match = 0; for (int k = 0; k < values.Length; k++)
            {
                var j = i + k; if (j >= source.Length) break;
                var x = source[j];
                var y = values[k];
                if (comparer.Equals(x, y)) match++; else break;
            }
            if (match == values.Length) return i;
        }
        return -1;
    }
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array contains the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOf(values) >= 0;

    /// <summary>
    /// Determines if the array contains the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOf(values, comparer) >= 0;

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array starts with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => (source.Length == 0 && values.Length == 0)
        || (source.IndexOf(values, comparer) == 0);
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array ends with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => (source.Length == 0 && values.Length == 0)
        || (source.Length > 0
        && source.IndexOf(values, comparer) == source.Length - values.Length);
#endif

    /*

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(item, value)) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(item, value)) return i;
        }
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexesOfAny(values, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        List<int> nums = []; for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values) if (comparer.Equals(value, item))
            {
                nums.Add(i);
                break;
            }
        }
        return nums;
    }

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.ContainsAny(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array starts with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOfAny(values) == 0;

    /// <summary>
    /// Determines if the array starts with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array ends with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.Length > 0 && source.IndexOfAny(values) == source.Length - 1;

    /// <summary>
    /// Determines if the array ends with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0 && source.IndexOfAny(values, comparer) == source.Length - 1;

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAnyExcept(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            var found = false;

            foreach (var value in values)
            {
                var except = comparer.Equals(item, value);
                if (except) { found = true; break; }
            }
            if (!found) return i;
        }
        return -1;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAnyExcept(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var item = source[i];
            var found = false;

            foreach (var value in values)
            {
                var except = comparer.Equals(item, value);
                if (except) { found = true; break; }
            }
            if (!found) return i;
        }
        return -1;
    }
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of all values that are NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexesOfAnyExcept(values, comparer);
    }

    /// <summary>
    /// Returns the indexes of all values that are NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        List<int> nums = [];
        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            var found = false;

            foreach (var value in values)
            {
                var except = comparer.Equals(item, value);
                if (except) { found = true; break; }
            }
            if (!found) nums.Add(i);
        }
        return nums;
    }

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array contains any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOfAnyExcept(values) >= 0;
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array contains any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAnyExcept(values, comparer) >= 0;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array starts with any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWithAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOfAnyExcept(values) == 0;

    /// <summary>
    /// Determines if the array starts with any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWithAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAnyExcept(values, comparer) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array ends with any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWithAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.Length > 0
        && source.IndexOfAnyExcept(values) == source.Length - 1;

    /// <summary>
    /// Determines if the array ends with any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWithAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0
        && source.IndexOfAnyExcept(values, comparer) == source.Length - 1;
     */
}
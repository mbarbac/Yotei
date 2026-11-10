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
    /// Determines if the span shall be considered equal to the values' sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool SequenceEqual<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.SequenceEqual(values, comparer);
    }
#endif

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
            var result = comparer.Equals(xvalue, yvalue); if (!result) return false;
        }
        return true;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines the order of the span relative to the sequence of values.
    /// <br/>- Less than 0: span precedes the values' sequence.
    /// <br/>- Equal to 0: span equals the values' sequence.
    /// <br/>- Greater than 0: span follows the values' sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int SequenceCompareTo<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = ComparerEx.New<T>();
        return source.SequenceCompareTo(values, comparer);
    }
#endif

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
            var result = comparer.Compare(xvalue, yvalue); if (result != 0) return result;
        }
        return 0;
    }
#endif

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the first index of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAny(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the first index of any of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
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
    /// Searches for the last index of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAny(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the last index of any of the given values, using the given element's comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
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
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
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
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span starts with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.StartsWithAny(values, comparer);
    }

    /// <summary>
    /// Determines if the span starts with any of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length > 0)
        {
            var item = source[0];
            foreach (var value in values) if (comparer.Equals(item, value)) return true;
        }
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span ends with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.EndsWithAny(values, comparer);
    }

    /// <summary>
    /// Determines if the span ends with any of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        if (source.Length > 0)
        {
            var item = source[^1];
            foreach (var value in values) if (comparer.Equals(item, value)) return true;
        }
        return false;
    }

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first value that IS NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAnyExcept(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first value that IS NOT any of the given ones, using the given
    /// elements' comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

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
    /// Returns the index of the last value that IS NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAnyExcept(values, comparer);
    }
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last value that IS NOT any of the given ones, using the given
    /// elements' comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

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
    /// Removes from the span all leading ocurrences of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method shadows the standard one because it what restricted to <see cref="IEquatable{T}"/>
    /// types, but all other methods do not.
    /// </remarks>
    public static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimStart(values, comparer);
    }

    /// <summary>
    /// Removes from the span all leading ocurrences of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TrimStart<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var span = source; if (values.Length > 0)
        {
            while (span.Length > 0)
            {
                var item = span[0];
                var found = false;

                foreach (var value in values)
                    if (comparer.Equals(item, value)) { found = true; break; }

                if (found) span = span[1..];
                else break;
            }
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the span all trailing ocurrences of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method shadows the standard one because it what restricted to <see cref="IEquatable{T}"/>
    /// types, but all other methods do not.
    /// </remarks>
    public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimEnd(values, comparer);
    }

    /// <summary>
    /// Removes from the span all trailing ocurrences of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TrimEnd<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var span = source; if (values.Length > 0)
        {
            while (span.Length > 0)
            {
                var item = span[^1];
                var found = false;

                foreach (var value in values)
                    if (comparer.Equals(item, value)) { found = true; break; }

                if (found) span = span[..^1];
                else break;
            }
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the span all leading and trailing ocurrences of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> Trim<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> values)
    {
        return source.TrimStart(values).TrimEnd(values);
    }

    /// <summary>
    /// Removes from the span all leading and trailing ocurrences of the given values, using the
    /// given elements' comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> Trim<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        return source.TrimStart(values, comparer).TrimEnd(values, comparer);
    }
}
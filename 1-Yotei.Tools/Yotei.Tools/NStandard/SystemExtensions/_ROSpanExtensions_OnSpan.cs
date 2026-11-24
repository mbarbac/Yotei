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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given sequence of values, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        // mimic standard behavior...
        if (source.Length == 0 && values.Length == 0) return 0;

        if (source.IsEmpty) return -1;
        if (values.IsEmpty) return -1;
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given sequence of values, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        // mimic standard behavior...
        if (source.Length == 0 && values.Length == 0) return 0;

        if (source.IsEmpty) return -1;
        if (values.IsEmpty) return -1;
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

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span contains the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOf(values) >= 0;

    /// <summary>
    /// Determines if the span contains the given sequence of values.
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

    /// <summary>
    /// Determines if the span starts with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOf(values, comparer) == 0;

    /// <summary>
    /// Determines if the span ends with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.Length == 0 && values.Length == 0
        || source.Length > 0 && source.LastIndexOf(values, comparer) == source.Length - values.Length;

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the first ocurrence of any of the given values.
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

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the last ocurrence of any of the given values.
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

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOfAny(values) >= 0;
#endif

    /// <summary>
    /// Determines if the span contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;

    // ----------------------------------------------------

    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.Length > 0 && source.IndexOfAny(values) == 0;

    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0 && source.IndexOfAny(values, comparer) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span ends with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.Length > 0 && source.IndexOfAny(values) == source.Length - 1;

    /// <summary>
    /// Determines if the span ends with any of the given values.
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
    /// Returns the index of the first value that is not any of the given ones.
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

    /// <summary>
    /// Returns the index of the first value that is not any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
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

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last value that is not any of the given ones.
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

    /// <summary>
    /// Returns the index of the last value that is not any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
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

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span contains any values except the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        => source.IndexOfAnyExcept(values) >= 0;
#endif

    /// <summary>
    /// Determines if the span contains any values except the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this ReadOnlySpan<T> source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAnyExcept(values, comparer) >= 0;
}
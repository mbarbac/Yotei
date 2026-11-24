#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

/*
 *  IndexOfAnyRange
 *  LastIndexOfAnyRange
 *  IndexesOfAnyRange
 *  ContainsAnyRange
 *  StartsWithAnyRange
 *  EndsWithAnyRange
 */
// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class ReadOnlySpanExtensions
{
    /// <summary>
    /// Invoked to obtain a span from the given enumerable.
    /// <br/> If an array is returned in the out argument, it must be returned to the pool.
    /// </summary>
    /// <returns></returns>
    static ReadOnlySpan<T> ToSpan<T>(this IEnumerable<T> source, out T[]? buffer)
    {
        buffer = null;

        // Easiest case...
        if (source is T[] array) return array;

        // The 2nd easiest case...
        if (source is ArraySegment<T> segment)
        {
            return new ReadOnlySpan<T>(segment.Array, segment.Offset, segment.Count);
        }

        // Source is a list (and we can marshall)...
        if (source is List<T> list)
        {
#if !NETSTANDARD2_0
            return CollectionsMarshal.AsSpan(list);
#endif
        }

        // We can know the size upfront...
        if (source is ICollection<T> collection)
        {
            var count = collection.Count;
            if (count == 0) return [];

            buffer = ArrayPool<T>.Shared.Rent(count);
            collection.CopyTo(buffer, 0);
            return new ReadOnlySpan<T>(buffer, 0, count);
        }

        // Allocating a span of an arbitrary size...
        else
        {
            buffer = ArrayPool<T>.Shared.Rent(16);
            int index = 0;

            foreach (T item in source)
            {
                if (index == buffer.Length)
                {
                    var exp = ArrayPool<T>.Shared.Rent(buffer.Length * 2);
                    Array.Copy(buffer, 0, exp, 0, index);
                    ArrayPool<T>.Shared.Return(buffer, true);
                    buffer = exp;
                }
                buffer[index++] = item;
            }
            return new ReadOnlySpan<T>(buffer, 0, index);
        }
    }

    // ====================================================

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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given sequence of values, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOf(values, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given sequence of values, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = values.ToSpan(out var buffer);
        try { return source.IndexOf(span, comparer); }
        finally { if (buffer is not null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given sequence of values, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOf(values, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given sequence of values, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = values.ToSpan(out var buffer);
        try { return source.LastIndexOf(span, comparer); }
        finally { if (buffer is not null) ArrayPool<T>.Shared.Return(buffer, true); }
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
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.Contains(values, comparer);
    }

    /// <summary>
    /// /// Determines if the span contains the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = values.ToSpan(out var buffer);
        try { return source.Contains(span, comparer); }
        finally { if (buffer is not null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span starts with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.StartsWith(values, comparer);
    }

    /// <summary>
    /// Determines if the span starts with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = values.ToSpan(out var buffer);
        try { return source.StartsWith(span, comparer); }
        finally { if (buffer is not null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span ends with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.EndsWith(values, comparer);
    }

    /// <summary>
    /// Determines if the span ends with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = values.ToSpan(out var buffer);
        try { return source.EndsWith(span, comparer); }
        finally { if (buffer is not null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the first ocurrence of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAny(values, comparer);
    }

    /// <summary>
    /// Searches for the first ocurrence of any of the given values.
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

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the last ocurrence of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAny(values, comparer);
    }

    /// <summary>
    /// Searches for the last ocurrence of any of the given values.
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

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
        => source.IndexOfAny(values) >= 0;

    /// <summary>
    /// Determines if the span contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span ends with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
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
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0 && source.IndexOfAny(values, comparer) == source.Length - 1;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first value that is not any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAnyExcept(values, comparer);
    }

    /// <summary>
    /// Returns the index of the first value that is not any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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

    /// <summary>
    /// Returns the index of the last value that is not any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAnyExcept(values, comparer);
    }

    /// <summary>
    /// Returns the index of the last value that is not any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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

    /// <summary>
    /// Determines if the span contains any values except the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) where T : IEquatable<T>
        => source.IndexOfAnyExcept(values) >= 0;

    /// <summary>
    /// Determines if the span contains any values except the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAnyExcept(values, comparer) >= 0;
}
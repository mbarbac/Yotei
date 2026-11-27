#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

using System.Buffers;

// ========================================================
public static partial class ArrayExtension
{
    /// Returns the index of the first ocurrence of the given sequence, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOf(values, comparer);
    }

    /// Returns the index of the first ocurrence of the given sequence, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(comparer);

        var span = values.ToSpan(out var buffer);
        try { return source.IndexOf(span, comparer); }
        finally { if (buffer != null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// Returns the index of the last ocurrence of the given sequence, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOf(values, comparer);
    }

    /// Returns the index of the last ocurrence of the given sequence, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(comparer);

        var span = values.ToSpan(out var buffer);
        try { return source.LastIndexOf(span, comparer); }
        finally { if (buffer != null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array contains the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOf(values, comparer) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array starts with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
        => source.IndexOf(values) == 0;

    /// <summary>
    /// Determines if the array starts with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOf(values, comparer) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array ends with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.EndsWith(values, comparer);
    }

    /// <summary>
    /// Determines if the array ends with the given sequence of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        // Because we need 'values.Length' to compare against, we need to capture the temporal
        // buffer in the method instead of relying in the standard logic, which would have been
        // the 'return' portion below.

        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(comparer);

        var span = values.ToSpan(out var buffer);
        try
        {
            return
                source.Length > 0 &&
                source.IndexOf(span, comparer) == source.Length - span.Length;
        }
        finally { if (buffer != null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAny(values, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAny(values, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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
    /// Returns the indexes of the ocurrences of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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

    /// <summary>
    /// Determines if the array contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.ContainsAny(values, comparer);
    }

    /// <summary>
    /// Determines if the array contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAny(values, comparer) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array starts with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0 && source.IndexOfAny(values, comparer) == source.Length - 1;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexOfAnyExcept(values, comparer);
    }

    /// <summary>
    /// Returns the index of the first value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(comparer);

        var span = values.ToSpan(out var buffer);
        try { return source.IndexOfAnyExcept(span, comparer); }
        finally { if (buffer != null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.LastIndexOfAnyExcept(values, comparer);
    }

    /// <summary>
    /// Returns the index of the last value that is NOT any of the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAnyExcept<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(comparer);

        var span = values.ToSpan(out var buffer);
        try { return source.LastIndexOfAnyExcept(span, comparer); }
        finally { if (buffer != null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

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
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(comparer);

        var span = values.ToSpan(out var buffer);
        try { return source.IndexesOfAnyExcept(span, comparer); }
        finally { if (buffer != null) ArrayPool<T>.Shared.Return(buffer, true); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array contains any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
        => source.IndexOfAnyExcept(values) >= 0;

    /// <summary>
    /// Determines if the array contains any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAnyExcept<T>(
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.IndexOfAnyExcept(values, comparer) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array starts with any value EXCEPT the given ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWithAnyExcept<T>(
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, IEnumerable<T> values) where T : IEquatable<T>
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
        this T[] source, IEnumerable<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0
        && source.IndexOfAnyExcept(values, comparer) == source.Length - 1;
}
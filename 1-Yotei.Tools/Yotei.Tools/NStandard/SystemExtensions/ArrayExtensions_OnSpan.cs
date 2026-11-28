#if NETSTANDARD2_0
using System.Xml.Serialization;

namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class ArrayExtension
{
    /// <summary>
    /// Returns the index of the first ocurrence of any of the given values, or -1 if none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
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
        this T[] source, ReadOnlySpan<T> values) where T : IEquatable<T>
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
        this T[] source, ReadOnlySpan<T> values, IEqualityComparer<T> comparer)
        => source.Length > 0
        && source.IndexOfAnyExcept(values, comparer) == source.Length - 1;
}
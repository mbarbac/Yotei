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
    /// Returns the index of the first ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.IndexOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the index of the first value that matches the given predicate, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++) if (predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.LastIndexOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the index of the last value that matches the given predicate, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = source.Length -1; i >= 0; i--) if (predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this ReadOnlySpan<T> source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexesOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.IndexesOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the indexes of the values that match the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> nums = [];
        for (int i = 0; i < source.Length; i++) if (predicate(source[i])) nums.Add(i);
        return nums;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span contains the given value.
    /// 
    /// Returns the index of the first ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the span contains the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;
}
/* 
 *  StartsWith
 *  EndsWith
 *  
 *  TryFind
 *  TryFindLast
 *  TryFindAll
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
 *  
 *  Add(value)
 *  AddRange(span)
 *  AddRange(sequence)
 *  Insert(value)
 *  InsertRange(span)
 *  InsertRange(sequence)
 *  
 *  RemoveAt(index)
 *  RemoveRange(index, count)
 *  Remove(value)
 *  RemoveLast(value)
 *  RemoveAll(value)
 *  Remove(predicate)
 *  RemoveLast(predicate)
 *  RemoveAll(predicate)
 *  Clear()
 */
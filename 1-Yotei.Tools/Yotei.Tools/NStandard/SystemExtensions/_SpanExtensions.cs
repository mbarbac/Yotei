#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

/*  TODO: SpanExtensions: Add, AddRange, ...
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
// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class SpanExtensions
{
    /// <summary>
    /// Returns a new instance where all leading ocurrences of the default value have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimStart(comparer);
    }

    /// <summary>
    /// Returns a new instance where all leading ocurrences of the default value have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(
        this Span<T> source, IEqualityComparer<T> comparer)
        => source.TrimStart(default!, comparer);

    /// <summary>
    /// Returns a new instance where all leading ocurrences of the given value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(
        this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.TrimStart(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a new instance where all leading ocurrences of the given value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var result = ((ReadOnlySpan<T>)source).TrimStart(predicate);

        int start = (int)(Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(source),
            ref MemoryMarshal.GetReference(result))) / Unsafe.SizeOf<T>();

        return source[start..(start + result.Length)];
    }
}
/*

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the default value have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(this Span<T> source) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimEnd(comparer);
    }

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the default value have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(
        this Span<T> source, IEqualityComparer<T> comparer)
        => source.TrimEnd(default!, comparer);

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the given value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(
        this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.TrimEnd(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the given value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var span = source; while (span.Length > 0)
        {
            if (predicate(span[^1])) span = span[..^1];
            else break;
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the default value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source) where T : IEquatable<T>
        => source.TrimStart().TrimEnd();

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the default value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source, IEqualityComparer<T> comparer)
        => source.TrimStart(comparer).TrimEnd(comparer);

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source, T value, IEqualityComparer<T> comparer)
        => source.TrimStart(value, comparer).TrimEnd(value, comparer);

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source, Predicate<T> predicate)
        => source.TrimStart(predicate).TrimEnd(predicate);
}*/
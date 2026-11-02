#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#pragma warning disable IDE0079
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static class SpanExtensions
{
    /// <summary>
    /// Returns a copy of this instance where all the leading ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(
        this Span<T> source) => source.TrimStart(default!, EqualityComparer<T>.Default);

    /// <summary>
    /// Returns a copy of this instance where all the leading ocurrences of the given value have
    /// been removed, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.TrimStart(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a copy of this instance where all the leading ocurrences of values that match the
    /// given predicate have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var span = source; while (true)
        {
            if (span.Length == 0) break;
            if (predicate(span[0])) span = span[1..];
            else break;
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where all the trailing ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(
        this Span<T> source) => source.TrimEnd(default!, EqualityComparer<T>.Default);

    /// <summary>
    /// Returns a copy of this instance where all the trailing ocurrences of the given value have
    /// been removed, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.TrimEnd(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a copy of this instance where all the trailing ocurrences of values that match the
    /// given predicate have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var span = source; while (true)
        {
            if (span.Length == 0) break;
            if (predicate(span[^1])) span = span[..^1];
            else break;
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where all the leading and trailing ocurrences of the given
    /// value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source) => source.Trim(default!, EqualityComparer<T>.Default);

    /// <summary>
    /// Returns a copy of this instance where all the leading and trailing ocurrences of the given
    /// value have been removed, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.Trim(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a copy of this instance where all the leading and trailing ocurrences of values
    /// that match the given predicate have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source, Predicate<T> predicate) => source.TrimStart(predicate).TrimEnd(predicate);

    // ----------------------------------------------------
    // Replace
}
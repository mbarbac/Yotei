#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

// Cannot use extension blocks because any restrictions on 'T' at that level apply to all methods,
// and we have methods with different restrictions.
namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class SpanExtensions
{
    /// <summary>
    /// Removes from the span all leading ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source, T value)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimStart(value, comparer);
    }

    /// <summary>
    /// Removes from the span all leading ocurrences of the given value, using the given elements'
    /// comparer.
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
    /// Removes from the span all leading ocurrences of elements that match the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var span = source; while (span.Length > 0)
        {
            if (predicate(span[0])) span = span[1..];
            else break;
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the span all tralining ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(this Span<T> source, T value)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimEnd(value, comparer);
    }

    /// <summary>
    /// Removes from the span all trailing ocurrences of the given value, using the given elements'
    /// comparer.
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
    /// Removes from the span all trailing ocurrences of elements that match the given predicate.
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
    /// Removes from the span all leading and tralining ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(this Span<T> source, T value)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimStart(value, comparer).TrimEnd(value, comparer);
    }

    /// <summary>
    /// Removes from the span all leading and trailing ocurrences of the given value, using the
    /// given elements' comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(
        this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        return source.TrimStart(value, comparer).TrimEnd(value, comparer);
    }

    /// <summary>
    /// Removes from the span all leading and trailing ocurrences of elements that match the given
    /// predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(this Span<T> source, Predicate<T> predicate)
    {
        return source.TrimStart(predicate).TrimEnd(predicate);
    }
}
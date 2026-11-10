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
static partial class SpanExtensions
{
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
    public static Span<T> TrimStart<T>(this Span<T> source, Span<T> values)
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
    public static Span<T> TrimStart<T>(
        this Span<T> source, Span<T> values, IEqualityComparer<T> comparer)
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
    public static Span<T> TrimEnd<T>(this Span<T> source, Span<T> values)
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
    public static Span<T> TrimEnd<T>(
        this Span<T> source, Span<T> values, IEqualityComparer<T> comparer)
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
    public static Span<T> Trim<T>(this Span<T> source, Span<T> values)
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
    public static Span<T> Trim<T>(
        this Span<T> source, Span<T> values, IEqualityComparer<T> comparer)
    {
        return source.TrimStart(values, comparer).TrimEnd(values, comparer);
    }
}
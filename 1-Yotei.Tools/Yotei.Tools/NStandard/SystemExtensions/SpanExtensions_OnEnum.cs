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
    /// Removes from the span all leading ocurrences of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(1)]
    public static Span<T> TrimStart<T>(
        this Span<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = source; while (span.Length > 0)
        {
            var item = span[0];
            var found = false;

            foreach (var value in values)
                if (comparer.Equals(item, value)) { found = true; break; }

            if (found) span = span[1..];
            else break;
        }

        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the span all trailing ocurrences of the given values, using the given elements'
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(1)]
    public static Span<T> TrimEnd<T>(
        this Span<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var span = source; while (span.Length > 0)
        {
            var item = span[^1];
            var found = false;

            foreach (var value in values)
                if (comparer.Equals(item, value)) { found = true; break; }

            if (found) span = span[..^1];
            else break;
        }

        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the span all leading and trailing ocurrences of the given values, using the
    /// given elements' comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(1)]
    public static Span<T> Trim<T>(
        this Span<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        return source.TrimStart(values, comparer).TrimEnd(values, comparer);
    }
}
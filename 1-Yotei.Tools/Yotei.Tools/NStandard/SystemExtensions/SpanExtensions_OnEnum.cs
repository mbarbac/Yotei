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
    /// Returns a new instance where the values from the given range have been added to the
    /// original one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Span<T> AddRange<T>(this Span<T> source, IEnumerable<T> values)
    {
        var buffer = values.ToSpan(out var array);
        try
        {
            var target = new T[source.Length + buffer.Length];
            var i = 0;
            var k = 0;

            for (; i < source.Length; i++) target[i] = source[i];
            for (; k < buffer.Length; k++) target[i++] = buffer[k];
            return target;
        }
        finally { array?.TryReturnToPool(); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given range of values have been inserted into the
    /// original one starting at the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> InsertRange<T>(this Span<T> source, int index, IEnumerable<T> values)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index cannot be negative.").WithData(index);

        if (index > source.Length) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index cannot be bigger than length.").WithData(index);

        var buffer = values.ToSpan(out var array);
        try
        {
            var target = new T[source.Length + buffer.Length];

            int i = 0;
            for (; i < index; i++) target[i] = source[i];
            
            var n = i;
            int k = 0;            
            for (; k < buffer.Length; k++) target[i++] = buffer[k];
            for (; n < source.Length; n++) target[i++] = source[n];

            return target;
        }
        finally { array?.TryReturnToPool(); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given sequence of values, if any,
    /// has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Span<T> Remove<T>(this Span<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.Remove(values, comparer);
    }

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given sequence of values, if any,
    /// has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Remove<T>(this Span<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var buffer = values.ToSpan(out var array);
        try
        {
            var index = source.IndexOf(values, comparer);
            return index >= 0 ? source.RemoveRange(index, buffer.Length) : source.Clone();
        }
        finally { array?.TryReturnToPool(); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given sequence of values, if any,
    /// has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Span<T> RemoveLast<T>(this Span<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.RemoveLast(values, comparer);
    }

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given sequence of values, if any,
    /// has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> RemoveLast<T>(this Span<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var buffer = values.ToSpan(out var array);
        try
        {
            var index = source.LastIndexOf(values, comparer);
            return index >= 0 ? source.RemoveRange(index, buffer.Length) : source.Clone();
        }
        finally { array?.TryReturnToPool(); }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given sequence of values, if any,
    /// have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Span<T> RemoveAll<T>(this Span<T> source, IEnumerable<T> values) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.RemoveAll(values, comparer);
    }

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given sequence of values, if any,
    /// have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> RemoveAll<T>(
        this Span<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();

        var buffer = values.ToSpan(out var array);
        try
        {
            if (source.Length == 0) return source.Clone();
            do
            {
                var index = source.IndexOf(values, comparer);
                if (index < 0) return source.Clone();

                source = source.RemoveRange(index, buffer.Length);
            }
            while (source.Length > 0);
            return source;
        }
        finally { array?.TryReturnToPool(); }
    }
}
#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class SpanExtensions
{
    /// <summary>
    /// Returns a new instance that is a clone of the original one. Its elements are either the
    /// original ones or its deep clones as requested.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="deep"></param>
    /// <returns></returns>
    public static Span<T> Clone<T>(this Span<T> source, bool deep = false)
    {
        var span = source.ToArray(); if (deep)
        {
            for (int i = 0; i < source.Length; i++) span[i] = source[i].TryClone()!;
        }
        return span;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all leading ocurrences of the default value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(this Span<T> source)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimStart(default!, comparer);
    }

    /// <summary>
    /// Returns a new instance where all leading ocurrences of the given value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> TrimStart<T>(
        this Span<T> source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimStart(value, comparer);
    }

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
        ArgumentNullException.ThrowIfNull(comparer);

        while (source.Length > 0)
        {
            if (comparer.Equals(source[0], value)) source = source[1..];
            else break;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the default value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(this Span<T> source)
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimEnd(default!, comparer);
    }

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the given value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> TrimEnd<T>(
        this Span<T> source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.TrimEnd(value, comparer);
    }

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
        ArgumentNullException.ThrowIfNull(comparer);

        while (source.Length > 0)
        {
            if (comparer.Equals(source[^1], value)) source = source[..^1];
            else break;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the default value
    /// have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(this Span<T> source) => source.TrimStart().TrimEnd();

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(this Span<T> source, T value) where T : IEquatable<T>
        => source.TrimStart(value).TrimEnd(value);

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Trim<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
        => source.TrimStart(value, comparer).TrimEnd(value, comparer);
}
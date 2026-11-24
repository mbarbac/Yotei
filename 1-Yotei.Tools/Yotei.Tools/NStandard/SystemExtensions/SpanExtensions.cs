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
    /// Returns a new instance that is a clone of the current one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Span<T> Clone<T>(this Span<T> source)
    {
        var target = new T[source.Length];
        for (int i = 0; i < source.Length; i++) target[i] = source[i];
        return target;
    }

    // ----------------------------------------------------

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

        var result = ((ReadOnlySpan<T>)source).TrimEnd(predicate);

        int start = (int)(Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(source),
            ref MemoryMarshal.GetReference(result))) / Unsafe.SizeOf<T>();

        return source[start..(start + result.Length)];
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given value has been added to the original one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> Add<T>(this Span<T> source, T value)
    {
        var target = new T[source.Length + 1];
        for (int i = 0; i < source.Length; i++) target[i] = source[i];
        target[source.Length] = value;
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given value has been inserted into the original one at
    /// the given index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> Insert<T>(this Span<T> source, int index, T value)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index cannot be negative.").WithData(index);

        if (index > source.Length) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index cannot be bigger than length.").WithData(index);

        var target = new T[source.Length + 1];
        int i = 0;

        for (; i < index; i++) target[i] = source[i];
        target[index] = value;
        for (; i < source.Length; i++) target[i + 1] = source[i];
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Span<T> RemoveAt<T>(this Span<T> source, int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index cannot be negative.").WithData(index);

        if (index >= source.Length) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index bigger than length.").WithData(index);

        if (source.Length == 0) return [];

        var target = new T[source.Length - 1];

        int i = 0;
        for (; i < index; i++) target[i] = source[i];
        
        int n = i + 1;
        for (; n < source.Length; n++) target[i++] = source[n];
        
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed from the original collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Span<T> RemoveRange<T>(this Span<T> source, int index, int count)
    {
        if (index == 0 && count == 0 && source.Length == 0) return []; // Special case...

        if (index < 0) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index cannot be negative.").WithData(index);

        if (index >= source.Length) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Index bigger than length.").WithData(index);

        if (count < 0) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Count cannot be negative.").WithData(index);

        if (count > source.Length) throw new ArgumentOutOfRangeException(
            nameof(index),
            "Count bigger than length.").WithData(index);

        if ((index + count) > source.Length) throw new ArgumentOutOfRangeException(
           nameof(index),
           "Index plus count bigger than length.").WithData(index).WithData(count);

        var target = new T[source.Length - count];
        
        int i = 0;
        for (; i < index; i++) target[i] = source[i];

        var n = i + count;
        for (; n < source.Length; n++) target[i++] = source[n];
        return target;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given value, if any, has been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> Remove<T>(this Span<T> source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.Remove(value, comparer);
    }

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given value, if any, has been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> Remove<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.Remove(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a new instance where the first ocurrence of a value that matches the given
    /// predicate, if any, has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> Remove<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var index = source.IndexOf(predicate);
        return index >= 0 ? source.RemoveAt(index) : source.Clone();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given value, if any, has been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> RemoveLast<T>(this Span<T> source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.RemoveLast(value, comparer);
    }

    /// <summary>
    /// Returns a new instance where the last ocurrence of the given value, if any, has been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> RemoveLast<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.RemoveLast(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a new instance where the last ocurrence of a value that matches the given
    /// predicate, if any, has been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> RemoveLast<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var index = source.LastIndexOf(predicate);
        return index >= 0 ? source.RemoveAt(index) : source.Clone();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given value, if any, have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<T> RemoveAll<T>(this Span<T> source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.RemoveAll(value, comparer);
    }

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given value, if any, have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Span<T> RemoveAll<T>(this Span<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.RemoveAll(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns a new instance where all the ocurrences of the given value, if any, have been
    /// removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Span<T> RemoveAll<T>(this Span<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var index = source.IndexOf(predicate); if (index < 0) return source.Clone();
        while (index >= 0)
        {
            source = source.RemoveAt(index);
            index = source.IndexOf(predicate);
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the elements have been set to the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Span<T> Clear<T>(this Span<T> source) => new T[source.Length];
}
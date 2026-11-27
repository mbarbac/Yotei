#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static partial class ArrayExtension
{
    extension<T>(T[] source)
    {
        /// <summary>
        /// Gets a value indicating whether the source sequence is empty.
        /// </summary>
        public bool IsEmpty => source.Length == 0;

        // ------------------------------------------------

        /// <summary>
        /// Obtains a typed enumerator for the contents of the array.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetTypedEnumerator() => ((IEnumerable<T>)source).GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// Returns a clone of this instance where its elements are either the original ones, or
        /// their deep clones, as requested.
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public T[] Clone(bool deep)
        {
            if (deep) // Deep cloning requested...
            {
                var len = source.Length;
                var result = new T[len];

                for (int i = 0; i < len; i++) result[i] = source[i].TryClone()!;
                return result;
            }
            else // Standard case...
            {
                return (T[])source.Clone();
            }
        }
    }

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return source.IndexOf(x => comparer.Equals(x, value));
    }
#endif

    /// <summary>
    /// Returns the index of the first value that matches the given predicate, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i < source.Length; i++) if (predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return source.LastIndexOf(x => comparer.Equals(x, value));
    }
#endif

    /// <summary>
    /// Returns the index of the last value that matches the given predicate, or -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i])) return i;
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
    public static List<int> IndexesOf<T>(this T[] source, T value) where T : IEquatable<T>
    {
        var comparer = EqualityComparerEx.New<T>();
        return source.IndexesOf(value, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        return source.IndexesOf(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Returns the indexes of values that match the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(this T[] source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        List<int> nums = [];
        for (int i = 0; i < source.Length; i++) if (predicate(source[i])) nums.Add(i);
        return nums;
    }

    // ----------------------------------------------------

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the array contains the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this T[] source, T value, IEqualityComparer<T> comparer)
        => source.IndexOf(value, comparer) >= 0;
#endif

    /// <summary>
    /// Determines if the array contains a value that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this T[] source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array starts with the given value, or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this T[] source, T value) where T : IEquatable<T>
        => source.IndexOf(value) == 0;

    /// <summary>
    /// Determines if the array starts with the given value, or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this T[] source, T value, IEqualityComparer<T> comparer)
        => source.IndexOf(value, comparer) == 0;

    /// <summary>
    /// Determines if the array starts with a value that matches the given predictae, or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this T[] source, Predicate<T> predicate)
        => source.IndexOf(predicate) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the array ends with the given value, or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this T[] source, T value) where T : IEquatable<T>
        => source.Length > 0 && source.IndexOf(value) == source.Length - 1;

    /// <summary>
    /// Determines if the array ends with the given value, or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this T[] source, T value, IEqualityComparer<T> comparer)
        => source.Length > 0 && source.IndexOf(value, comparer) == source.Length - 1;

    /// <summary>
    /// Determines if the array ends with a value that matches the given predictae, or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this T[] source, Predicate<T> predicate)
        => source.Length > 0 && source.IndexOf(predicate) == source.Length - 1;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all leading ocurrences of the default value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] TrimStart<T>(this T[] source)
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
    public static T[] TrimStart<T>(this T[] source, T value) where T : IEquatable<T>
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
    public static T[] TrimStart<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        var span = source.AsSpan(); while (span.Length > 0)
        {
            if (comparer.Equals(span[0], value)) span = span[1..];
            else break;
        }
        return span.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all trailing ocurrences of the default value have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] TrimEnd<T>(this T[] source)
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
    public static T[] TrimEnd<T>(this T[] source, T value) where T : IEquatable<T>
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
    public static T[] TrimEnd<T>(this T[] source, T value, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        var span = source.AsSpan(); while (span.Length > 0)
        {
            if (comparer.Equals(span[^1], value)) span = span[..^1];
            else break;
        }
        return span.ToArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the default value
    /// have been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] Trim<T>(this T[] source) => source.TrimStart().TrimEnd();

    /// <summary>
    /// Returns a new instance where all leading and trailing ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T[] Trim<T>(this T[] source, T value) where T : IEquatable<T>
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
    public static T[] Trim<T>(this T[] source, T value, IEqualityComparer<T> comparer)
        => source.TrimStart(value, comparer).TrimEnd(value, comparer);
}
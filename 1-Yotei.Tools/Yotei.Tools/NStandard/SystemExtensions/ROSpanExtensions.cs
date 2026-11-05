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
static partial class ReadOnlySpanExtensions
{
#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the first ocurrence of the given value.
    /// <br/> '<c>T</c>' must implement <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method returns false instead of throwing an exception to mimin modern C# behavior.
    /// </remarks>
    [SuppressMessage("", "IDE0075")]
    public static int IndexOf<T>(this ReadOnlySpan<T> source, T value)
    {
        return source.IndexOf(x =>
            value is IComparable<T> comparable ? comparable.CompareTo(x) == 0 :
            value is IEquatable<T> equatable ? equatable.Equals(x) :
            false);
    }
#endif

    /// <summary>
    /// Returns the index of the first ocurrence of the given value, using the given comparer.
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
    /// Returns the index of the first value that matches the given predicate.
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

#if NETSTANDARD2_0
    /// <summary>
    /// Returns the index of the last ocurrence of the given value.
    /// <br/> '<c>T</c>' must implement <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method returns false instead of throwing an exception to mimin modern C# behavior.
    /// </remarks>
    [SuppressMessage("", "IDE0075")]
    public static int LastIndexOf<T>(this ReadOnlySpan<T> source, T value)
    {
        return source.LastIndexOf(x =>
            value is IComparable<T> comparable ? comparable.CompareTo(x) == 0 :
            value is IEquatable<T> equatable ? equatable.Equals(x) :
            false);
    }
#endif

    /// <summary>
    /// Returns the index of the last ocurrence of the given value, using the given comparer.
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
    /// Returns the index of the last value that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the ocurrences of the given value.
    /// <br/> '<c>T</c>' must implement <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0075")]
    public static List<int> IndexesOf<T>(this ReadOnlySpan<T> source, T value)
    {
        return source.IndexesOf(x =>
            value is IComparable<T> comparable ? comparable.CompareTo(x) == 0 :
            value is IEquatable<T> equatable ? equatable.Equals(x) :
            false);
    }

    /// <summary>
    /// Returns the index of the ocurrences of the given value, using the given comparer.
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

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span contains the given value.
    /// <br/> '<c>T</c>' must implement <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, T value) => source.IndexOf(value) >= 0;
#endif

#if NETSTANDARD2_0
    /// <summary>
    /// Determines if the span contains the given value, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
        => source.IndexOf(value, comparer) >= 0;
#endif

    /// <summary>
    /// Determines if the span contains any values that match the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;
}
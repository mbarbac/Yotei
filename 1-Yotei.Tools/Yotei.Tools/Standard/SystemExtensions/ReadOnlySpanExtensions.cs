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
static class ReadOnlySpanExtensions
{
    /// <summary>
    /// Determines if the two sequences can be considered the same by comparing their respective
    /// elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method is made available to .NET (in addition to .NET Standard) because we want the
    /// compiler to resolve to this method instead of the .NET one. Reason is that by doing so, it
    /// can support 'T' types that need not to implement 'IComparable'.
    /// </remarks>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values) => source.SequenceCompareTo(values) == 0;

    /// <summary>
    /// Determines if the two sequences can be considered the same by comparing their respective
    /// elements with the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method is made available to .NET (in addition to .NET Standard) because we want the
    /// compiler to resolve to this method instead of the .NET one. Reason is that by doing so, it
    /// can support 'T' types that need not to implement 'IComparable'.
    /// </remarks>
    public static bool SequenceEqual<T>(
        this ReadOnlySpan<T> source,
        IEnumerable<T> values, IComparer<T> comparer) => source.SequenceCompareTo(values, comparer) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// Determines the relative order of the two sequences by comparing their elements.
    /// <br/>- If less than 0, <c>source</c> precedes <c>values</c>.
    /// <br/>- If 0, <c>source</c> equals <c>values</c>.
    /// <br/>- If greater than cero, <c>values</c> follow <c>source</c>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method is made available to .NET (in addition to .NET Standard) because we want the
    /// compiler to resolve to this method instead of the .NET one. Reason is that by doing so, it
    /// can support 'T' types that need not to implement 'IComparable'.
    /// </remarks>
    public static int SequenceCompareTo<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values)
        => source.SequenceCompareTo(values, ComparerFactory.Create<T>());

    /// <summary>
    /// Determines the relative order of two sequences by comparing their elements with the given
    /// comparer.
    /// <br/>- If less than 0, <c>source</c> precedes <c>values</c>.
    /// <br/>- If 0, <c>source</c> equals <c>values</c>.
    /// <br/>- If greater than cero, <c>values</c> follow <c>source</c>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    /// <notes>
    /// This method MUST also be available to .NET in addition to :NET Standard. The reason is
    /// that we want the compiler to resolve to this method instead of the .NET one, because by
    /// doing so, we support 'T' types that need not to implement 'IComparable'.
    /// </notes>
    public static int SequenceCompareTo<T>(
        this ReadOnlySpan<T> source,
        IEnumerable<T> values, IComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        using var iter = values.GetEnumerator();
        for (int i = 0; i < source.Length; i++)
        {
            if (!iter.MoveNext()) return +1;
            var item = source[i];
            var temp = iter.Current;
            var comp = comparer.Compare(item, temp); if (comp != 0) return comp;
        }
        if (iter.MoveNext()) return -1;
        return 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this sequence contains the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, T value) => source.IndexOf(value) >= 0;

    /// <summary>
    /// Determines if this sequence contains the given value, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if this sequence constain any elements that match the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the index of the first ocurrence of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf<T>(
        this ReadOnlySpan<T> source, T value)
        => source.IndexOf(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the index of the first ocurrence of the given value, using the given comparer.
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
    /// Searches for the index of the first value that matches the given predicate.
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

    /// <summary>
    /// Searches for the index of the last ocurrence of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(
        this ReadOnlySpan<T> source, T value)
        => source.LastIndexOf(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the index of the last ocurrence of the given value, using the given comparer.
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
    /// Searches for the index of the last value that matches the given predicate.
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
    /// Searches for the indexes of all ocurrences of the given value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this ReadOnlySpan<T> source, T value)
        => source.IndexesOf(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the indexes of all ocurrences of the given value, using the given comparer.
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
    /// Searches for the indexes of the values that match the given predicate.
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

    /// <summary>
    /// Searches for the first index of any value other than the given one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, T value)
        => source.IndexOfAnyExcept(value, EqualityComparer<T>.Default);

#if NETSTANDARD2_0
    /// <summary>
    /// Searches for the first index of any value other than the given one, using the given
    /// comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.IndexOfAnyExcept(x => comparer.Equals(x, value));
    }
#endif

    /// <summary>
    /// Searches for the first index of any value other than the ones that match the given
    /// predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++) if (!predicate(source[i])) return i;
        return -1;
    }
}
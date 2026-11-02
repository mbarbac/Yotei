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
    /// Determines if the span contains the given value or not, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source,
        T value, IEqualityComparer<T> comparer) => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the span contains a value that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(
        this ReadOnlySpan<T> source, Predicate<T> predicate) => source.IndexOf(predicate) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the index of the first ocurrence of the given value, using the given comparer.
    /// Returns -1 if any.
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
    /// Returns -1 if any.
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
    /// Searches for the index of the last ocurrence of the given value, using the given comparer.
    /// Returns -1 if any.
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
    /// Returns -1 if any.
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
    /// Searches for the indexes of the ocurrences of the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this ReadOnlySpan<T> source, T value) => source.IndexesOf(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the indexes of the ocurrences of the given value, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf<T>(
        this ReadOnlySpan<T> source,
        T value, IEqualityComparer<T> comparer)
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
    /// Searches for the first index of any value EXCEPT the given one.
    /// Returns -1 if any or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int FirstCropIndex<T>(
        this ReadOnlySpan<T> source, T value) => source.FirstCropIndex(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the first index of any value EXCEPT the given one, using the given comparer.
    /// Returns -1 if any or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FirstCropIndex<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.FirstCropIndex(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Searches for the first index of any value EXCEPT the ones that matches the given predicate.
    /// Returns -1 if any or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int FirstCropIndex<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++) if (!predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Searches for the last index of any value EXCEPT the given one.
    /// Returns -1 if any or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastCropIndex<T>(
        this ReadOnlySpan<T> source, T value) => source.LastCropIndex(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the last index of any value EXCEPT the given one, using the given comparer.
    /// Returns -1 if any or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastCropIndex<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.LastCropIndex(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Searches for the last index of any value EXCEPT the ones that matches the given predicate.
    /// Returns -1 if any or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int LastCropIndex<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--) if (!predicate(source[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where all the leading ocurrences of the given value have
    /// been removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TrimStart<T>(
        this ReadOnlySpan<T> source) => source.TrimStart(default!, EqualityComparer<T>.Default);

    /// <summary>
    /// Returns a copy of this instance where all the leading ocurrences of the given value have
    /// been removed, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TrimStart<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
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
    public static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
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
    public static ReadOnlySpan<T> TrimEnd<T>(
        this ReadOnlySpan<T> source) => source.TrimEnd(default!, EqualityComparer<T>.Default);

    /// <summary>
    /// Returns a copy of this instance where all the trailing ocurrences of the given value have
    /// been removed, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> TrimEnd<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
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
    public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
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
    public static ReadOnlySpan<T> Trim<T>(
        this ReadOnlySpan<T> source) => source.Trim(default!, EqualityComparer<T>.Default);

    /// <summary>
    /// Returns a copy of this instance where all the leading and trailing ocurrences of the given
    /// value have been removed, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static ReadOnlySpan<T> Trim<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
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
    public static ReadOnlySpan<T> Trim<T>(
        this ReadOnlySpan<T> source, Predicate<T> predicate)
        => source.TrimStart(predicate).TrimEnd(predicate);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span starts with the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this ReadOnlySpan<T> source, T value) => source.StartsWith(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Determines if the span starts with the given value, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.StartsWith(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Determines if the span starts with a value that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return source.Length > 0 && source.IndexOf(predicate) == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span ends with the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this ReadOnlySpan<T> source, T value) => source.EndsWith(value, EqualityComparer<T>.Default);

    /// <summary>
    /// Determines if the span ends with the given value, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(
        this ReadOnlySpan<T> source, T value, IEqualityComparer<T> comparer)
    {
        comparer.ThrowWhenNull();
        return source.EndsWith(x => comparer.Equals(x, value));
    }

    /// <summary>
    /// Determines if the span ends with a value that matches the given predicate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return source.Length > 0 && source.IndexOf(predicate) == source.Length - 1;
    }

    // ----------------------------------------------------
#if NETSTANDARD2_0

    /// <summary>
    /// Determines if the span contains any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values)
        => source.IndexOfAny(values, EqualityComparer<T>.Default) >= 0;

    /// <summary>
    /// Determines if the span contains any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(
        this ReadOnlySpan<T> source,
        IEnumerable<T> values, IEqualityComparer<T> comparer) => source.IndexOfAny(values, comparer) >= 0;

#endif
    // ----------------------------------------------------
#if NETSTANDARD2_0

    /// <summary>
    /// Searches for the first index of any of the given values, using the given comparer.
    /// Returns -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values)
                if (comparer.Equals(value, item)) return i;
        }
        return -1;
    }

#endif
    // ----------------------------------------------------
#if NETSTANDARD2_0

    /// <summary>
    /// Searches for the last index of any of the given values, using the given comparer.
    /// Returns -1 if any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var item = source[i];
            foreach (var value in values)
                if (comparer.Equals(value, item)) return i;
        }
        return -1;
    }

#endif
    // ----------------------------------------------------

    /// <summary>
    /// Searches for the all indexes of any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values)
        => source.IndexesOfAny(values, EqualityComparer<T>.Default);

    /// <summary>
    /// Searches for the all indexes of any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOfAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        List<int> nums = []; for (int i = 0; i < source.Length; i++)
        {
            var item = source[i];
            foreach (var value in values)
                if (comparer.Equals(value, item)) { nums.Add(i); break; }
        }
        return nums;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span starts with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values)
        => source.StartsWithAny(values, EqualityComparer<T>.Default);

    /// <summary>
    /// Determines if the span starts with any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWithAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        if (source.Length == 0) return false;

        var item = source[0];
        foreach (var value in values) if (comparer.Equals(item, value)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the span ends with any of the given values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values)
        => source.EndsWithAny(values, EqualityComparer<T>.Default);

    /// <summary>
    /// Determines if the span ends with any of the given values, using the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWithAny<T>(
        this ReadOnlySpan<T> source, IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        values.ThrowWhenNull();
        comparer.ThrowWhenNull();

        if (source.Length == 0) return false;

        var item = source[^1];
        foreach (var value in values) if (comparer.Equals(item, value)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to find the first ocurrence of a value in the span that matches the given predicate.
    /// If found, sets the out argument to it, and returns true. Otherwise, returns false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryFind<T>(this ReadOnlySpan<T> source, Predicate<T> predicate, out T value)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i];
            if (predicate(temp))
            {
                value = temp;
                return true;
            }
        }
        value = default!;
        return false;
    }

    /// <summary>
    /// Tries to find the last ocurrence of a value in the span that matches the given predicate.
    /// If found, sets the out argument to it, and returns true. Otherwise, returns false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryFindLast<T>(this ReadOnlySpan<T> source, Predicate<T> predicate, out T value)
    {
        predicate.ThrowWhenNull();

        for (int i = source.Length - 1; i >= 0; i--)
        {
            var temp = source[i];
            if (predicate(temp))
            {
                value = temp;
                return true;
            }
        }
        value = default!;
        return false;
    }

    /// <summary>
    /// Tries to find all the ocurrences of values in the span that match the given predicate.
    /// Sets the out argument to the collection of found ones. Returns true if any is found,
    /// otherwise, returns false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryFindAll<T>(
        this ReadOnlySpan<T> source, Predicate<T> predicate, out List<T> values)
    {
        predicate.ThrowWhenNull();

        values = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i];
            if (predicate(temp)) values.Add(temp);
        }
        return values.Count > 0;
    }
}
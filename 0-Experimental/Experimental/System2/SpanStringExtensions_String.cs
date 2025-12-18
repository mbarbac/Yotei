using StringSpan = System.ReadOnlySpan<char>;

namespace Experimental;

// ========================================================
public static class SpanStringExtensions_String
{
    // span.StartsWith(Span)
    // span.StartsWith(Span, StringComparison)
    // span.StartsWith(Span, IEqualityComparer<char>)

    static bool StartsWith(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        for (int i = 0; i < value.Length; i++)
        {
            var s = source[i];
            var v = value[i];
            if (!predicate(s, v)) return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    // span.EndsWith(Span)
    // span.EndsWith(Span, StringComparison)
    // span.EndsWith(Span, IEqualityComparer<char>)

    static bool EndsWith(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        int ini = source.Length - value.Length;
        for (int i = 0; i < value.Length; i++)
        {
            var s = source[ini + i];
            var v = value[i];
            if (!predicate(s, v)) return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    // span.IndexOf(Span)
    // span.IndexOf(Span, StringComparison)
    // span.IndexOf(Span, IEqualityComparer<char>)

    static int IndexOf(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (StartsWith(temp, value, predicate)) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    public static int IndexOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    // span.LastIndexOf(Span)
    // span.LastIndexOf(Span, StringComparison)
    // span.LastIndexOf(Span, IEqualityComparer<char>)

    static int LastIndexOf(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var index = -1;
        for (int i = 0; i <= source.Length - value.Length; i++)
        {
            var temp = source[i..];
            if (StartsWith(temp, value, predicate)) index = i;
        }
        return index;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    static List<int> IndexesOf(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];
        if (value.Length > source.Length) return [];

        List<int> list = []; for (int i = 0; i < source.Length; i++)
        {
            var temp = source[i..];
            if (StartsWith(temp, value, predicate)) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value)
        => IndexesOf(source, value, static (x, y) => x == y);

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => IndexesOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.Contains(Span, StringComparison)

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value) => source.IndexOf(value) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => source.IndexOf(value, ignoreCase) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the characters from the given index till the last
    /// position have been deleted.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0057")]
    public static StringSpan Remove(
        this StringSpan source, int index) => source.ToString().Remove(index);

    /// <summary>
    /// Returns a new instance in which the given number of characters starting at the given index
    /// have been deleted.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static StringSpan Remove(this StringSpan source, int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index + count, source.Length);

        if (count == 0) return source;
        return source.ToString().Remove(index, count);
    }

    // ----------------------------------------------------

    static StringSpan Remove(
        StringSpan source, StringSpan value, Func<char, char, bool> predicate, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;
        if (value.Length > source.Length) return source;

        var index = IndexOf(source, value, predicate);
        if (index >= 0) source = source.Remove(index, value.Length);

        removed = index >= 0;
        return source;
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value)
        => Remove(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, out bool removed)
        => Remove(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => Remove(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => Remove(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static StringSpan RemoveLast(
        StringSpan source, StringSpan value, Func<char, char, bool> predicate, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;
        if (value.Length > source.Length) return source;

        var index = LastIndexOf(source, value, predicate);
        if (index >= 0) source = source.Remove(index, value.Length);

        removed = index >= 0;
        return source;
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value)
        => RemoveLast(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, out bool removed)
        => RemoveLast(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => RemoveLast(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    public static StringSpan RemoveAll(
        StringSpan source, StringSpan value,
        Func<char, char, bool> predicate,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;
        if (value.Length == 0) return source;
        if (value.Length > source.Length) return source;

        while (true)
        {
            source = Remove(source, value, predicate, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value)
        => RemoveAll(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, out bool removed)
        => RemoveAll(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => RemoveAll(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static int HeadIndex(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var index = IndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = index - 1; i >= 0; i--)
                if (source[index] != ' ') return -1;
        }
        return index;
    }

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int HeadIndex(
        StringSpan source, StringSpan value) => HeadIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int HeadIndex(
        StringSpan source, StringSpan value, bool ignoreCase)
        => HeadIndex(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int HeadIndex(
        StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => HeadIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int HeadIndex(
        StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => HeadIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int HeadIndex(
        StringSpan source, StringSpan value, StringComparison comparison)
        => HeadIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static int TailIndex(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var index = LastIndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = index + value.Length; i < source.Length; i++)
                if (source[i] != ' ') return -1;
        }
        return index;
    }

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the tailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int TailIndex(
        StringSpan source, StringSpan value) => TailIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the tailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int TailIndex(
        StringSpan source, StringSpan value, bool ignoreCase)
        => TailIndex(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the tailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int TailIndex(
        StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => TailIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the tailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int TailIndex(
        StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => TailIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the tailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int TailIndex(
        StringSpan source, StringSpan value, StringComparison comparison)
        => TailIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static StringSpan RemoveHead(
        this StringSpan source, StringSpan value,
        Func<char, char, bool> predicate,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0 && value.Length == 0) return source;
        if (source.Length == 0 || value.Length == 0) return source;
        if (value.Length > source.Length) return source;

        var index = HeadIndex(source, value, predicate);
        if (index >= 0) { source = source.Remove(index, value.Length); removed = true; }
        return source;
    }

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value)
        => RemoveHead(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, out bool removed)
        => RemoveHead(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, bool ignoreCase)
        => RemoveHead(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
        => RemoveHead(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, StringComparison comparison)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveHead(
        StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static StringSpan RemoveTail(
        this StringSpan source, StringSpan value,
        Func<char, char, bool> predicate,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0 && value.Length == 0) return source;
        if (source.Length == 0 || value.Length == 0) return source;
        if (value.Length > source.Length) return source;

        var index = TailIndex(source, value, predicate);
        if (index >= 0) { source = source.Remove(index, value.Length); removed = true; }
        return source;
    }

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value)
        => RemoveTail(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, out bool removed)
        => RemoveTail(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, bool ignoreCase)
        => RemoveTail(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, bool ignoreCase, out bool removed)
        => RemoveTail(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, IEqualityComparer<char> comparer, out bool removed)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, IEqualityComparer<string> comparer, out bool removed)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, StringComparison comparison)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveTail(
        StringSpan source, StringSpan value, StringComparison comparison, out bool removed)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparison), out removed);
}
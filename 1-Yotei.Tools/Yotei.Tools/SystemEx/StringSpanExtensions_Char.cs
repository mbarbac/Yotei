namespace Yotei.Tools;

// ========================================================
public static class SpanStringExtensions_Char
{
    // span.StartsWith(char)
    // span.StartsWith(char, IEqualityComparer<char>)

    static bool StartsWith(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return false;
        return predicate(source[0], value);
    }

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, bool ignoreCase)
        => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, StringComparison comparison)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.EndsWith(char)
    // span.EndsWith(char, IEqualityComparer<char>)

    static bool EndsWith(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return false;
        return predicate(source[^1], value);
    }

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, bool ignoreCase)
        => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, StringComparison comparison)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.IndexOf(char)
    // span.IndexOf(char, IEqualityComparer<char>)

    static int IndexOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++) if (predicate(source[i], value)) return i;
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
        this StringSpan source, char value, bool ignoreCase)
        => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int IndexOf(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    public static int IndexOf(
        this StringSpan source, char value, StringComparison comparison)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.LastIndexOf(char)
    // span.LastIndexOf(char, IEqualityComparer<char>)

    static int LastIndexOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        var index = -1;
        for (int i = 0; i < source.Length; i++) if (predicate(source[i], value)) index = i;
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
        this StringSpan source, char value, bool ignoreCase)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    public static int LastIndexOf(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the source, or -1 if it
    /// cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    public static int LastIndexOf(
        this StringSpan source, char value, StringComparison comparison)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static List<int> IndexesOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return [];

        List<int> list = [];
        for (int i = 0; i < source.Length; i++) if (predicate(source[i], value)) list.Add(i);
        return list;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value)
        => IndexesOf(source, value, static (x, y) => x == y);

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, bool ignoreCase)
        => IndexesOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value in the source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, StringComparison comparison)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.Contains(char)
    // span.Contains(char, IEqualityComparer<char>)

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, bool ignoreCase)
        => source.IndexOf(value, ignoreCase) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => source.IndexOf(value, comparer) >= 0;

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value, StringComparison comparison)
        => source.IndexOf(value, comparison) >= 0;

    // ----------------------------------------------------

    // span.IndexOfAny(SearchValues<char> values)
    // span.IndexOfAny(StringSpan values)
    // span.IndexOfAny(StringSpan values, IEqualityComparer<char>)
    // span.IndexOfAny(char, char)
    // span.IndexOfAny(char, char, IEqualityComparer<char>)
    // span.IndexOfAny(char, char, char)
    // span.IndexOfAny(char, char, char, IEqualityComparer<char>)

    static int IndexOfAny(
        StringSpan source, IEnumerable<char> values, Func<char, char, bool> predicate)
    {
        foreach (var item in values)
        {
            var index = IndexOf(source, item, predicate);
            if (index >= 0) return index;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the given source contains any of the given values and, if so, returns the
    /// index of the first match. Otherwise, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values)
        => IndexOfAny(source, values, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source contains any of the given values and, if so, returns the
    /// index of the first match. Otherwise, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, bool ignoreCase)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the given source contains any of the given values and, if so, returns the
    /// index of the first match. Otherwise, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source contains any of the given values and, if so, returns the
    /// index of the first match. Otherwise, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source contains any of the given values and, if so, returns the
    /// index of the first match. Otherwise, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, StringComparison comparison)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.ContainsAny(SearchValues<char> values)
    // span.ContainsAny(StringSpan values)
    // span.ContainsAny(StringSpan values, IEqualityComparer<char>)
    // span.ContainsAny(char, char)
    // span.ContainsAny(char, char, IEqualityComparer<char>)
    // span.ContainsAny(char, char, char)
    // span.ContainsAny(char, char, char, IEqualityComparer<char>)

    /// <summary>
    /// Determines if the given source contains any of the given values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values)
        => source.IndexOfAny(values) >= 0;

    /// <summary>
    /// Determines if the given source contains any of the given values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, bool ignoreCase)
        => source.IndexOfAny(values, ignoreCase) >= 0;

    /// <summary>
    /// Determines if the given source contains any of the given values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
        => source.IndexOfAny(values, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains any of the given values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
        => source.IndexOfAny(values, comparer) >= 0;

    /// <summary>
    /// Determines if the given source contains any of the given values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, StringComparison comparison)
        => source.IndexOfAny(values, comparison) >= 0;

    // ----------------------------------------------------

    static StringSpan Remove(
        StringSpan source, char value, Func<char, char, bool> predicate, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = IndexOf(source, value, predicate);
        if (index >= 0) source = source.Remove(index, 1);

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
        this StringSpan source, char value)
        => Remove(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, out bool removed)
        => Remove(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, bool ignoreCase, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, char value, StringComparison comparison)
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
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => Remove(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static StringSpan RemoveLast(
        StringSpan source, char value, Func<char, char, bool> predicate, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = LastIndexOf(source, value, predicate);
        if (index >= 0) source = source.Remove(index, 1);

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
        this StringSpan source, char value)
        => RemoveLast(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, out bool removed)
        => RemoveLast(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, bool ignoreCase, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, char value, StringComparison comparison)
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
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static StringSpan RemoveAll(
        StringSpan source, char value, Func<char, char, bool> predicate, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

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
        this StringSpan source, char value)
        => RemoveAll(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, out bool removed)
        => RemoveAll(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, bool ignoreCase, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, char value, StringComparison comparison)
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
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static int HeadIndex(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        var index = IndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = index - 1; i >= 0; i--) if (source[i] != ' ') return -1;
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
        this StringSpan source, char value) => HeadIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int HeadIndex(
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, StringComparison comparison)
        => HeadIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static int TailIndex(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        var index = LastIndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = index + 1; i < source.Length; i++) if (source[i] != ' ') return -1;
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
        this StringSpan source, char value) => TailIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the tailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int TailIndex(
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, StringComparison comparison)
        => TailIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static StringSpan RemoveHead(
        StringSpan source, char value,
        Func<char, char, bool> predicate,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = HeadIndex(source, value, predicate);
        if (index >= 0) { source = source.Remove(index, 1); removed = true; }
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
        this StringSpan source, char value)
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
        this StringSpan source, char value, out bool removed)
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
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, bool ignoreCase, out bool removed)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
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
        this StringSpan source, char value, StringComparison comparison)
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
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => RemoveHead(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static StringSpan RemoveTail(
        StringSpan source, char value,
        Func<char, char, bool> predicate,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = TailIndex(source, value, predicate);
        if (index >= 0) { source = source.Remove(index, 1); removed = true; }
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
        this StringSpan source, char value)
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
        this StringSpan source, char value, out bool removed)
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
        this StringSpan source, char value, bool ignoreCase)
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
        this StringSpan source, char value, bool ignoreCase, out bool removed)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer)
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
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer)
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
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
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
        this StringSpan source, char value, StringComparison comparison)
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
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => RemoveTail(source, value, (x, y) => x.Equals(y, comparison), out removed);
}
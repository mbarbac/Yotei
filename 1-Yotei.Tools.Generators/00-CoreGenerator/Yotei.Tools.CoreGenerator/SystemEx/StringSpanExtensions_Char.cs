using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class StringSpanExtensions
{
    static bool StartsWith(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return false;
        return predicate(source[0], value);
    }

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value)
        => StartsWith(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, bool ignoreCase)
        => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, char value, StringComparison comparison)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static bool EndsWith(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return false;
        return predicate(source[^1], value);
    }

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value)
        => EndsWith(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, bool ignoreCase)
        => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
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

    static int IndexOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++) if (predicate(source[i], value)) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, char value, bool ignoreCase)
        => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, char value, StringComparison comparison)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    // span.LastIndexOf(char)

    static int LastIndexOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i], value)) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, char value, bool ignoreCase)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
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
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value)
        => IndexesOf(source, value, static (x, y) => x == y);

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, bool ignoreCase)
        => IndexesOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));
    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, char value, StringComparison comparison)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value) => source.IndexOf(value) >= 0;

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
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => source.IndexOf(value, comparer) >= 0;

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

    // span.IndexOfAny(span)

    static int IndexOfAny(
        StringSpan source, IEnumerable<char> values, Func<char, char, bool> predicate)
    {
        foreach (var value in values)
        {
            var index = IndexOf(source, value, predicate);
            if (index >= 0) return index;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values)
        => IndexOfAny(source, values, static (x, y) => x == y);

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, bool ignoreCase)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOfAny(
        this StringSpan source, IEnumerable<char> values, StringComparison comparison)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static bool ContainsAny(
        StringSpan source, IEnumerable<char> values, Func<char, char, bool> predicate)
        => IndexOfAny(source, values, predicate) >= 0;

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values)
        => ContainsAny(source, values, static (x, y) => x == y);

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, bool ignoreCase)
        => ContainsAny(source, values, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the index of the first match of any of the given values, in order, in the given
    /// source, or -1 if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, StringComparison comparison)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparison));

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

    static int PseudoHeadIndex(
        StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        var index = IndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
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
    public static int PseudoHeadIndex(
        this StringSpan source, char value)
        => PseudoHeadIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int PseudoHeadIndex(
        this StringSpan source, char value, bool ignoreCase)
        => PseudoHeadIndex(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int PseudoHeadIndex(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => PseudoHeadIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int PseudoHeadIndex(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => PseudoHeadIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source starts with the given value, ignoring the heading spaces.
    /// If so, returns the index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int PseudoHeadIndex(
        this StringSpan source, char value, StringComparison comparison)
        => PseudoHeadIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static int PseudoTailIndex(
        StringSpan source, char value, Func<char, char, bool> predicate)
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
    /// Determines if the given source ends with the given value, ignoring the trailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int PseudoTailIndex(
        this StringSpan source, char value)
        => PseudoTailIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the trailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int PseudoTailIndex(
        this StringSpan source, char value, bool ignoreCase)
        => PseudoTailIndex(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the trailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int PseudoTailIndex(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => PseudoTailIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the trailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int PseudoTailIndex(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => PseudoTailIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the given source ends with the given value, ignoring the trailing spaces.
    /// If so, returns the index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int PseudoTailIndex(
        this StringSpan source, char value, StringComparison comparison)
        => PseudoTailIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static StringSpan RemovePseudoHead(
        StringSpan source, char value, Func<char, char, bool> predicate, out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = PseudoHeadIndex(source, value, predicate);
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
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value)
        => RemovePseudoHead(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, out bool removed)
        => RemovePseudoHead(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, bool ignoreCase)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, bool ignoreCase, out bool removed)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, StringComparison comparison)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes the given value from the head of the given source, ignoring the heading spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoHead(
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => RemovePseudoHead(source, value, (x, y) => x.Equals(y, comparison), out removed);

    // ----------------------------------------------------

    static StringSpan RemovePseudoTail(
        StringSpan source, char value,
        Func<char, char, bool> predicate,
        out bool removed)
    {
        removed = false;
        if (source.Length == 0) return source;

        var index = PseudoTailIndex(source, value, predicate);
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
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value)
        => RemovePseudoTail(source, value, static (x, y) => x == y, out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, out bool removed)
        => RemovePseudoTail(source, value, static (x, y) => x == y, out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, bool ignoreCase)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, ignoreCase), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, bool ignoreCase, out bool removed)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, ignoreCase), out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, comparer), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, comparer), out removed);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, StringComparison comparison)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, comparison), out _);

    /// <summary>
    /// Removes the given value from the tail of the given source, ignoring the tailing spaces.
    /// Returns the original source is no removal was made.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StringSpan RemovePseudoTail(
        this StringSpan source, char value, StringComparison comparison, out bool removed)
        => RemovePseudoTail(source, value, (x, y) => x.Equals(y, comparison), out removed);
}
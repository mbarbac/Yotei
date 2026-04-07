using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
public static partial class StringSpanExtensions
{
    static int IndexOf(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        for (int i = 0; i < source.Length; i++)
        {
            var found = true;
            for (int j = 0; j < value.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!predicate(s, v)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// For whatever reasons the built-in method does not behave the right way, so hence why I'm
    /// adding this extension method for coherence reasons.
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    static int LastIndexOf(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        for (int i = source.Length - value.Length; i >= 0; i--)
        {
            var found = true;
            for (int j = 0; j < value.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!predicate(s, v)) { found = false; break; }
            }
            if (found) return i;
        }
        return -1;
    }

    /// <summary>
    ///  Returns the index of the last ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// For whatever reasons the built-in method does not behave the right way, so hence why I'm
    /// adding this extension method for coherence reasons.
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static List<int> IndexesOf(
        StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return [0];
        if (source.Length == 0 || value.Length == 0) return [];
        if (value.Length > source.Length) return [];

        List<int> list = [];
        for (int i = 0; i < source.Length; i++)
        {
            var found = true;
            for (int j = 0; j < value.Length; j++)
            {
                var s = source[i + j];
                var v = value[j];
                if (!predicate(s, v)) { found = false; break; }
            }
            if (found) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value)
        => IndexesOf(source, value, static (x, y) => x == y);

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => IndexesOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Returns the indexes of all ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => IndexesOf(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, StringSpan value)
        => source.IndexOf(value) >= 0;

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

    static bool StartsWith(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        for (int i = 0; i < value.Length; i++)
        {
            var schar = source[i];
            var vchar = value[i];
            if (!predicate(schar, vchar)) return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => StartsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source starts with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => StartsWith(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    static bool EndsWith(StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return true;
        if (source.Length == 0 || value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        var index = IndexOf(source, value, predicate);
        return index == source.Length - value.Length;
    }

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    // ----------------------------------------------------

    static int IndexOfAny(
        StringSpan source, IEnumerable<string> values, Func<char, char, bool> predicate)
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
        this StringSpan source, IEnumerable<string> values)
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
        this StringSpan source, IEnumerable<string> values, bool ignoreCase)
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
        this StringSpan source, IEnumerable<string> values, IEqualityComparer<char> comparer)
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
        this StringSpan source, IEnumerable<string> values, IEqualityComparer<string> comparer)
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
        this StringSpan source, IEnumerable<string> values, StringComparison comparison)
        => IndexOfAny(source, values, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static bool ContainsAny(
        StringSpan source, IEnumerable<string> values, Func<char, char, bool> predicate)
        => IndexOfAny(source, values, predicate) >= 0;

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<string> values)
        => ContainsAny(source, values, static (x, y) => x == y);

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<string> values, bool ignoreCase)
        => ContainsAny(source, values, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<string> values, IEqualityComparer<char> comparer)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<string> values, IEqualityComparer<string> comparer)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<string> values, StringComparison comparison)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new span where all the original characters from the given index, included, have
    /// been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static StringSpan Remove(this StringSpan source, int index) => source[..index];

    /// <summary>
    /// Returns a new span where the specific number of characters, staring at the given index,
    /// included, have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, int index, int count) => source.ToString().Remove(index, count);

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
    /// <returns></returns>
    public static StringSpan Remove(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => Remove(source, value, (x, y) => x.Equals(y, comparer), out _);

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
    /// <returns></returns>
    public static StringSpan RemoveLast(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveLast(source, value, (x, y) => x.Equals(y, comparer), out _);

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

    // ----------------------------------------------------

    static StringSpan RemoveAll(
        StringSpan source, StringSpan value, Func<char, char, bool> predicate, out bool removed)
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
    /// <returns></returns>
    public static StringSpan RemoveAll(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => RemoveAll(source, value, (x, y) => x.Equals(y, comparer), out _);

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

    // ----------------------------------------------------

    static int SnippedIndex(
        StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var index = IndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = 0; i < index; i++) if (source[i] != ' ') return -1;
        }
        return index;
    }

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int SnippedIndex(
        this StringSpan source, StringSpan value)
        => SnippedIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int SnippedIndex(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => SnippedIndex(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int SnippedIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => SnippedIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int SnippedIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => SnippedIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the first ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int SnippedIndex(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => SnippedIndex(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

    static int LastSnippedIndex(
        StringSpan source, StringSpan value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0 && value.Length == 0) return 0;
        if (source.Length == 0 || value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var index = LastIndexOf(source, value, predicate);
        if (index >= 0)
        {
            for (int i = index + value.Length; i < source.Length; i++)
            {
                if (source[i] != ' ') return -1;
            }
        }
        return index;
    }

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastSnippedIndex(
        this StringSpan source, StringSpan value)
        => LastSnippedIndex(source, value, static (x, y) => x == y);

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static int LastSnippedIndex(
        this StringSpan source, StringSpan value, bool ignoreCase)
        => LastSnippedIndex(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastSnippedIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<char> comparer)
        => LastSnippedIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastSnippedIndex(
        this StringSpan source, StringSpan value, IEqualityComparer<string> comparer)
        => LastSnippedIndex(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source starts with the given value, discarding any heading spaces. If
    /// so, returns the actual index of the last ocurrence of the value, or -1 otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastSnippedIndex(
        this StringSpan source, StringSpan value, StringComparison comparison)
        => LastSnippedIndex(source, value, (x, y) => x.Equals(y, comparison));
}
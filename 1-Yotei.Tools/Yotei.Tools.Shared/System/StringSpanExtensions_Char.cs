using StringSpan = System.ReadOnlySpan<char>;

#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_COREGENERATOR
internal
#else
public
#endif
static class StringSpanExtensions_OnChars
{
    static int IndexOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        for (int i = 0; i < source.Length; i++) if (predicate(source[i], value)) return i;
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
        this StringSpan source, char value, bool ignoreCase)
        => IndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

#if YOTEI_TOOLS_COREGENERATOR
    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
    /// it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => IndexOf(source, value, (x, y) => x.Equals(y, comparer));
#endif

    /// <summary>
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
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
    ///  Returns the index of the first ocurrence of the given value in the given source, or -1 if
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

    static int LastIndexOf(StringSpan source, char value, Func<char, char, bool> predicate)
    {
        if (source.Length == 0) return -1;

        for (int i = source.Length - 1; i >= 0; i--) if (predicate(source[i], value)) return i;
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
        this StringSpan source, char value, bool ignoreCase)
        => LastIndexOf(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    ///  Returns the index of the last ocurrence of the given value in the given source, or -1 if
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
    ///  Returns the index of the last ocurrence of the given value in the given source, or -1 if
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
    ///  Returns the index of the last ocurrence of the given value in the given source, or -1 if
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

#if YOTEI_TOOLS_COREGENERATOR
    /// <summary>
    /// Determines if the source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(
        this StringSpan source, char value) => source.IndexOf(value) >= 0;
#endif

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

#if YOTEI_TOOLS_COREGENERATOR
    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value)
        => EndsWith(source, value, static (x, y) => x == y);
#endif

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, bool ignoreCase)
        => EndsWith(source, value, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<char> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, IEqualityComparer<string> comparer)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if this source ends with the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool EndsWith(
        this StringSpan source, char value, StringComparison comparison)
        => EndsWith(source, value, (x, y) => x.Equals(y, comparison));

    // ----------------------------------------------------

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

#if YOTEI_TOOLS_COREGENERATOR
    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values)
        => ContainsAny(source, values, static (x, y) => x == y);
#endif

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, bool ignoreCase)
        => ContainsAny(source, values, (x, y) => x.Equals(y, ignoreCase));

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparer));

    /// <summary>
    /// Determines if the source contains any of the given values, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this StringSpan source, IEnumerable<char> values, StringComparison comparison)
        => ContainsAny(source, values, (x, y) => x.Equals(y, comparison));
}
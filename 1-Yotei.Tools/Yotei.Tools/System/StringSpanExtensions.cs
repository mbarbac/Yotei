using StrSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools;

// ========================================================
public static class StringSpanExtensions
{
    // ----------------------------------------------------
    // IndexOf(char)

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // LastIndexOf(char)

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, IEqualityComparer<string> comparer) => throw null;

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // IndexOf(span)
    // IndexOf(span, comparison)

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    // ----------------------------------------------------
    // LastIndexOf(span)
    // LastIndexOf(span, comparison)

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this StrSpan source, StrSpan value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // Contains(char)

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // Contains(span, comparison)

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, StrSpan value)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    // ----------------------------------------------------
    // ContainsAny(char, char)
    // ContainsAny(char, char, char)

    /// <summary>
    /// Determines if the given source contains any of the given values or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, IEnumerable<char> values, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // ContainsAny(span)

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, StrSpan values, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, StrSpan values, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, StrSpan values, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(this StrSpan source, StrSpan values, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // StartsWith(char)

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // StartsWith(span)
    // StartsWith(span, comparison)

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    // ----------------------------------------------------
    // EndsWith(char)

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------
    // EndsWith(span)
    // EndsWith(span, comparison)

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all characters starting from the given index have been
    /// removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, int index)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the requested number of characters starting from the given
    /// index have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, int index, int count)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, bool sensitive, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, StringComparison comparison, out bool removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, bool sensitive, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, StringComparison comparison, out bool removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, bool sensitive, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, StringComparison comparison, out bool removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, char value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, bool sensitive, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, StringComparison comparison, out bool removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan Remove(this StrSpan source, StrSpan value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, bool sensitive, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, StringComparison comparison, out bool removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveLast(this StrSpan source, StrSpan value, StringComparison comparison)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, bool sensitive, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer, out bool removed)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, StringComparison comparison, out bool removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, bool sensitive)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, IEqualityComparer<char> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, IEqualityComparer<string> comparer)
    {
        throw null;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static StrSpan RemoveAll(this StrSpan source, StrSpan value, StringComparison comparison)
    {
        throw null;
    }
}
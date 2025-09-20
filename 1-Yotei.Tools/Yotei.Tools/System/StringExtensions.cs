namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given source string is null or empty after being trimmed, if such has
    /// been requested (by default). Otherwise, returns either the original string or the trimmed
    /// one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim = true)
    {
        if (source is null) return null;

        if (trim)
        {
            var span = source.AsSpan().Trim();
            if (span.Length == 0) return null;
            if (span.Length == source.Length) return source;

            source = span.ToString();
        }

        return source.Length == 0 ? null : source;
    }

    /// <summary>
    /// Throws an <see cref="EmptyException"/> if the given source string is null or empty, either
    /// by itself or, by default, after being trimmed. Returns either the original string or the
    /// trimmed one, if such was requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? source,
        bool trim = true,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        description = description.NullWhenEmpty() ?? nameof(description);

        source.ThrowWhenNull();
        source = source.NullWhenEmpty(trim);
        if (source is null || source.Length == 0) throw new EmptyException(description);

        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new string where the diacritics of the original characters have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string source)
    {
        source.ThrowWhenNull();

        var temp = source.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in temp)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
        }

        return sb.ToString().Normalize();
    }

    // ----------------------------------------------------
    // IndexOf(char)
    // IndexOf(char, comparison)

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char value, bool sensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(value, sensitive);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(value, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(value, comparer);
    }

    // ----------------------------------------------------
    // LastIndexOf(char)
    // LastIndexOf(char, comparison)

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, char value, bool sensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value, sensitive);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this string source, char value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this string source, char value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value, comparer);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this string source, char value)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this string source, char value, bool sensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, sensitive);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this string source, char value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this string source, char value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this string source, char value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, comparison);
    }

    // ----------------------------------------------------
    // IndexOf(string)
    // IndexOf(string, comparison)

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, bool sensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexOf(value, sensitive);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexOf(value, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexOf(value, comparer);
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
    public static int LastIndexOf(this string source, string value, bool sensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value, sensitive);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(
        this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value, comparer);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this string source, string value)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this string source, string value, bool sensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, sensitive);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(
        this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, comparer);
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given value in the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<int> IndexesOf(this string source, string value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexesOf(value, comparison);
    }

    // ----------------------------------------------------
    // Contains(char)
    // Contains(char, comparison)
    // Contains(char, char comparer) - Enumerable, Linq

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool Contains(this string source, char value, bool sensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().Contains(value, sensitive);
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this string source, char value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().Contains(value, comparer);
    }

    // ----------------------------------------------------
    // Contains(string)
    // Contains(string, comparison)

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool Contains(this string source, string value, bool sensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Contains(value, sensitive);
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Contains(value, comparer);
    }

    /// <summary>
    /// Determines if the given source contains the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool Contains(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Contains(value, comparer);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source contains any of the given values or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> values)
    {
        source.ThrowWhenNull();
        values.ThrowWhenNull();
        return source.AsSpan().ContainsAny(values);
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> values, bool sensitive)
    {
        source.ThrowWhenNull();
        values.ThrowWhenNull();
        return source.AsSpan().ContainsAny(values, sensitive);
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> values, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        values.ThrowWhenNull();
        return source.AsSpan().ContainsAny(values, comparer);
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> values, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        values.ThrowWhenNull();
        return source.AsSpan().ContainsAny(values, comparer);
    }

    /// <summary>
    /// Determines if the given source span contains any character from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="values"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> values, StringComparison comparison)
    {
        source.ThrowWhenNull();
        values.ThrowWhenNull();
        return source.AsSpan().ContainsAny(values, comparison);
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
    public static bool StartsWith(this string source, char value, bool sensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, sensitive);
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this string source, char value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, comparer);
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this string source, char value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, comparer);
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool StartsWith(this string source, char value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, comparison);
    }

    // ----------------------------------------------------
    // StartsWith(string)
    // StartsWith(string, comparison)
    // StartsWith(string, ignoreCase, CultureInfo? culture)

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static bool StartsWith(this string source, string value, bool sensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, sensitive);
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, comparer);
    }

    /// <summary>
    /// Determines if the given source starts with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool StartsWith(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().StartsWith(value, comparer);
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
    public static bool EndsWith(this string source, char value, bool sensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, sensitive);
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this string source, char value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, comparer);
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this string source, char value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, comparer);
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool EndsWith(this string source, char value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, comparison);
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
    public static bool EndsWith(this string source, string value, bool sensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, sensitive);
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, comparer);
    }

    /// <summary>
    /// Determines if the given source ends with the given value or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool EndsWith(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().EndsWith(value, comparer);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the given number of characters (1 by default), starting from
    /// the given index.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string RemoveAt(this string source, int index, int count = 1)
    {
        source.ThrowWhenNull();

        var span = source.AsSpan().RemoveAt(index, count);
        return span.Length == source.Length ? source : span.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, char value, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, char value, bool sensitive, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, sensitive, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, StringComparison comparison, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, comparison, out removed);
        return removed ? item.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value) => source.Remove(value, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, bool sensitive) => source.Remove(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, char value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char value, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char value, bool sensitive, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, sensitive, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, StringComparison comparison, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, comparison, out removed);
        return removed ? item.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value) => source.RemoveLast(value, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, bool sensitive) => source.RemoveLast(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, IEqualityComparer<char> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, IEqualityComparer<string> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, char value, StringComparison comparison)
        => source.RemoveLast(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char value, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char value, bool sensitive, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, sensitive, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char value, IEqualityComparer<char> comparer, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char value, IEqualityComparer<string> comparer, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char value, StringComparison comparison, out bool removed)
    {
        source.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, comparison, out removed);
        return removed ? item.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, char value) => source.RemoveAll(value, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, char value, bool sensitive) => source.RemoveAll(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, char value, IEqualityComparer<char> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, char value, IEqualityComparer<string> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, char value, StringComparison comparison)
        => source.RemoveAll(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, bool sensitive, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, sensitive, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IEqualityComparer<char> comparer, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IEqualityComparer<string> comparer, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, StringComparison comparison, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().Remove(value, comparison, out removed);
        return removed ? item.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value) => source.Remove(value, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, bool sensitive) => source.Remove(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IEqualityComparer<char> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IEqualityComparer<string> comparer)
        => source.Remove(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the first ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, StringComparison comparison)
        => source.Remove(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, bool sensitive, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, sensitive, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, IEqualityComparer<char> comparer, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, IEqualityComparer<string> comparer, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, StringComparison comparison, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveLast(value, comparison, out removed);
        return removed ? item.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value) => source.RemoveLast(value, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, bool sensitive) => source.RemoveLast(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, IEqualityComparer<char> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, IEqualityComparer<string> comparer)
        => source.RemoveLast(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which the last ocurrence of the given value has been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, StringComparison comparison)
        => source.RemoveLast(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, bool sensitive, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, sensitive, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, IEqualityComparer<char> comparer, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, IEqualityComparer<string> comparer, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, comparer, out removed);
        return removed ? item.ToString() : source;
    }

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, StringComparison comparison, out bool removed)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var item = source.AsSpan().RemoveAll(value, comparison, out removed);
        return removed ? item.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value) => source.RemoveAll(value, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, bool sensitive) => source.RemoveAll(value, sensitive, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, IEqualityComparer<char> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, IEqualityComparer<string> comparer)
        => source.RemoveAll(value, comparer, out _);

    /// <summary>
    /// Returns a new instance in which all the ocurrences of the given value have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, StringComparison comparison)
        => source.RemoveAll(value, comparison, out _);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source starts with the given character ignoring any previous
    /// spaces. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int FindHeadIgnoreSpaces(
        this string source, char value)
        => source.ThrowWhenNull().AsSpan().FindHeadIgnoreSpaces(value);

    /// <summary>
    /// Determines if the given source starts with the given character ignoring any previous
    /// spaces. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int FindHeadIgnoreSpaces(
        this string source, char value, bool sensitive)
        => source.ThrowWhenNull().AsSpan().FindHeadIgnoreSpaces(value, sensitive);

    /// <summary>
    /// Determines if the given source starts with the given character ignoring any previous
    /// spaces. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FindHeadIgnoreSpaces(
        this string source, char value, IEqualityComparer<char> comparer)
        => source.ThrowWhenNull().AsSpan().FindHeadIgnoreSpaces(value, comparer);

    /// <summary>
    /// Determines if the given source starts with the given character ignoring any previous
    /// spaces. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FindHeadIgnoreSpaces(
        this string source, char value, IEqualityComparer<string> comparer)
        => source.ThrowWhenNull().AsSpan().FindHeadIgnoreSpaces(value, comparer);

    /// <summary>
    /// Determines if the given source starts with the given character ignoring any previous
    /// spaces. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int FindHeadIgnoreSpaces(
        this string source, char value, StringComparison comparison)
        => source.ThrowWhenNull().AsSpan().FindHeadIgnoreSpaces(value, comparison);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source end with the given character ignoring any spaces after
    /// it. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int FindTailIgnoreSpaces(
        this string source, char value)
        => source.ThrowWhenNull().AsSpan().FindTailIgnoreSpaces(value);

    /// <summary>
    /// Determines if the given source end with the given character ignoring any spaces after
    /// it. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static int FindTailIgnoreSpaces(
        this string source, char value, bool sensitive)
        => source.ThrowWhenNull().AsSpan().FindTailIgnoreSpaces(value, sensitive);

    /// <summary>
    /// Determines if the given source end with the given character ignoring any spaces after
    /// it. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FindTailIgnoreSpaces(
        this string source, char value, IEqualityComparer<char> comparer)
        => source.ThrowWhenNull().AsSpan().FindTailIgnoreSpaces(value, comparer);

    /// <summary>
    /// Determines if the given source end with the given character ignoring any spaces after
    /// it. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FindTailIgnoreSpaces(
        this string source, char value, IEqualityComparer<string> comparer)
        => source.ThrowWhenNull().AsSpan().FindTailIgnoreSpaces(value, comparer);

    /// <summary>
    /// Determines if the given source end with the given character ignoring any spaces after
    /// it. If so, returns the index of that first ocurrence. If not, returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int FindTailIgnoreSpaces(
        this string source, char value, StringComparison comparison)
        => source.ThrowWhenNull().AsSpan().FindTailIgnoreSpaces(value, comparison);
}
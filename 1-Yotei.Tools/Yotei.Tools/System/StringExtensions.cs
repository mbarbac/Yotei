namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given source string is null or empty, after trimming if such is
    /// requested. Otherwise, returns the resulting string.
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
        return string.IsNullOrWhiteSpace(source) ? null : source;
    }

    /// <summary>
    /// Throws an exception if the given source string is null or empty, after trimming it if
    /// such is requested. Otherwise returns the validated string.
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

        source = source.NullWhenEmpty(trim);
        source = source.ThrowWhenNull(description);
        return source!;
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

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(this string source, char c, bool caseSensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().Contains(c, caseSensitive);

    }

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(this string source, char c, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().Contains(c, comparer);
    }

    /// <summary>
    /// Determines if the given source contains the given char, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool Contains(this string source, char c, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().Contains(c, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array)
    {
        source.ThrowWhenNull();
        return source.AsSpan().ContainsAny(array);
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array, bool caseSensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().ContainsAny(array, caseSensitive);
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().ContainsAny(array, comparer);
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().ContainsAny(array, comparer);
    }

    /// <summary>
    /// Determines if the given source contains any char from the given array, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().ContainsAny(array, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char c, bool caseSensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(c, caseSensitive);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char c, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(c, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char c, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(c, comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, char c, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().IndexOf(c, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, char c, bool caseSensitive)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(c, caseSensitive);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, char c, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(c, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, char c, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(c, comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, char c, StringComparison comparison)
    {
        source.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(c, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexOf(value.AsSpan(), caseSensitive);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexOf(value.AsSpan(), comparer);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().IndexOf(value.AsSpan(), comparer);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value.AsSpan(), caseSensitive);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value.AsSpan(), comparer);
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given character in the given source,
    /// or -1 if it cannot be found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().LastIndexOf(value.AsSpan(), comparer);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Remove(value.AsSpan()).ToString();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Remove(value.AsSpan(), caseSensitive).ToString();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Remove(value.AsSpan(), comparer).ToString();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Remove(value.AsSpan(), comparer).ToString();
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Remove(value.AsSpan(), comparison).ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveLast(value.AsSpan()).ToString();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveLast(value.AsSpan(), caseSensitive).ToString();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveLast(value.AsSpan(), comparer).ToString();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveLast(value.AsSpan(), comparer).ToString();
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveLast(value.AsSpan(), comparison).ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveAll(value.AsSpan()).ToString();
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveAll(value.AsSpan(), caseSensitive).ToString();
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveAll(value.AsSpan(), comparer).ToString();
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveAll(value.AsSpan(), comparer).ToString();
    }

    /// <summary>
    /// Removes from the given source all the ocurrences of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().RemoveAll(value.AsSpan(), comparison).ToString();
    }
}
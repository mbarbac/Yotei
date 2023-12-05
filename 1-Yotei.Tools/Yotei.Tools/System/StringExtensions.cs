namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given string is null or empty, after trimming it if such is requested.
    /// Otherwise returns the given string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? value, bool trim = true)
    {
        if (value is not null && trim)
        {
            var span = value.AsSpan().Trim();

            if (span.Length == 0) return null;
            if (span.Length == value.Length) return value;
            value = span.IsWhiteSpace() ? null : span.ToString();
        }
        return value;
    }

    /// <summary>
    /// Throws an exception if the given string is null or empty, after trimming it if such is
    /// requested. Otherwise returns the validated string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? value,
        bool trim = true,
        [CallerArgumentExpression(nameof(value))] string? description = null)
    {
        description = description.NullWhenEmpty() ?? nameof(value);

        ArgumentNullException.ThrowIfNull(value, description);
        if (trim)
        {
            value = value.NullWhenEmpty(trim);
            ArgumentException.ThrowIfNullOrEmpty(value, description);
        }
        return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Wraps the given string with the given character, provided that it is not a null one after
    /// trimming, if such is requested, and that the source string was not already wrapped.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(
        this string? source, char ch, bool trim = true) => source.Wrap(ch, ch, trim);

    /// <summary>
    /// Wraps the given string with the given head and tail characters, provided that it is not
    /// a null one after trimming, if such is requested, and that the source string was not
    /// already wrapped.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(this string? source, char head, char tail, bool trim = true)
    {
        if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
        if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

        if (source != null)
        {
            if (trim) source = source.Trim();
            if (source.Length > 0)
            {
                if (source[0] != head &&
                    source[^1] != tail)
                    source = $"{head}{source}{tail}";
            }
        }

        return source;
    }

    /// <summary>
    /// Un-wraps the given string from the given character, provided that it is not a null one
    /// after trimming, if such is requested, and recursively if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? UnWrap(
        this string? source, char ch, bool trim = true, bool recursive = true)
        => source.UnWrap(ch, ch, trim, recursive);

    /// <summary>
    /// Un-wraps the given string from the given head and tail characters, provided that it is not
    /// a null one after trimming, if such is requested, and recursively if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? UnWrap(
        this string? source, char head, char tail, bool trim = true, bool recursive = true)
    {
        if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
        if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

        if (source != null)
        {
            var span = source.AsSpan();

            while (true)
            {
                if (trim) span = span.Trim();
                if (span.Length < 2) break;
                if (span[0] != head || span[^1] != tail) break;

                span = span[1..^1];
                if (!recursive) break;
            }

            source = span.ToString();
        }

        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given source contains any characters from the given array.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array)
    {
        source = source.ThrowWhenNull();
        array = array.ThrowWhenNull();

        foreach (var c in array) if (source.Contains(c)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any of the characters from the given array.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> array, StringComparison comparison)
    {
        source = source.ThrowWhenNull();
        array = array.ThrowWhenNull();

        foreach (var c in array) if (source.Contains(c, comparison)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given source contains any of the characters from the given array.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> array, IEqualityComparer<char> comparer)
    {
        source = source.ThrowWhenNull();
        array = array.ThrowWhenNull();
        comparer = comparer.ThrowWhenNull();

        foreach (var c in array) if (source.Contains(c, comparer)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new string where the diacritics of the original characters have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string source)
    {
        source = source.ThrowWhenNull();

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
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value)
    {
        return source.Remove(value, out _);
    }

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value)
    {
        return source.RemoveLast(value, out _);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, string value, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value)
    {
        return source.RemoveAll(value, out var _);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, string value, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, StringComparison comparison)
    {
        return source.Remove(value, comparison, out _);
    }

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, StringComparison comparison, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, StringComparison comparison)
    {
        return source.RemoveLast(value, comparison, out _);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, StringComparison comparison, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparison);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, StringComparison comparison)
    {
        return source.RemoveAll(value, comparison, out var _);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, StringComparison comparison, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparison, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IEqualityComparer<char> comparer)
    {
        return source.Remove(value, comparer, out _);
    }

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IEqualityComparer<char> comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, IEqualityComparer<char> comparer)
    {
        return source.RemoveLast(value, comparer, out _);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, IEqualityComparer<char> comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, IEqualityComparer<char> comparer)
    {
        return source.RemoveAll(value, comparer, out _);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, IEqualityComparer<char> comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IComparer<string> comparer)
    {
        return source.Remove(value, comparer, out _);
    }

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, IComparer<string> comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, IComparer<string> comparer)
    {
        return source.RemoveLast(value, comparer, out _);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, IComparer<string> comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = source.LastIndexOf(value, comparer);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, IComparer<string> comparer)
    {
        return source.RemoveAll(value, comparer, out _);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, IComparer<string> comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, Locale comparer)
    {
        return source.Remove(value, comparer, out _);
    }

    /// <summary>
    /// Removes the first ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string value, Locale comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = comparer.IndexOf(source, value);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, Locale comparer)
    {
        return source.RemoveLast(value, comparer, out _);
    }

    /// <summary>
    /// Removes the last ocurrence of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveLast(
        this string source, string value, Locale comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        var index = comparer.LastIndexOf(source, value);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, value.Length);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, Locale comparer)
    {
        return source.RemoveAll(value, comparer, out _);
    }

    /// <summary>
    /// Removes all the ocurrences of the given value from the given source string.
    /// Returns the resulting string, or the original one if the value was not found.
    /// The out argument indicates whether the value has been removed or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string RemoveAll(
        this string source, string value, Locale comparer, out bool removed)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();

        removed = false;
        if (source.Length == 0 || value.Length == 0) return source;

        while (true)
        {
            source = source.Remove(value, comparer, out var temp);
            if (!temp) break;
            removed = true;
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source string,
    /// or -1 if it was not found. If the value is empty, then the returned value is cero.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, IEqualityComparer<char> comparer)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();
        comparer = comparer.ThrowWhenNull();

        if (value.Length == 0) return 0;
        if (value.Length > source.Length) return -1;

        if (comparer is Locale locale) return locale.IndexOf(source, value);

        for (int s = 0; s < source.Length; s++)
        {
            for (int v = 0; v < value.Length; v++)
            {
                if (!comparer.Equals(source[s], value[v])) break;
                if (v == (value.Length - 1)) return s;
                v++;
                s++;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source string,
    /// or -1 if it was not found. If the value is empty, then the returned value is cero.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, string value, IEqualityComparer<char> comparer)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();
        comparer = comparer.ThrowWhenNull();

        if (value.Length == 0) return 0;
        if (value.Length > source.Length) return -1;

        if (comparer is Locale locale) return locale.IndexOf(source, value);

        for (int s = source.Length - 1; s >= 0; s--)
        {
            for (int v = value.Length - 1; v >= 0; v--)
            {
                if (!comparer.Equals(source[s], value[v])) break;
                if (v == 0) return s;
                v--;
                s--;
            }
        }

        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source string,
    /// or -1 if it was not found. If the value is empty, then the returned value is cero.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int IndexOf(this string source, string value, IComparer<string> comparer)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();
        comparer = comparer.ThrowWhenNull();

        if (value.Length == 0) return 0;
        if (value.Length > source.Length) return -1;

        if (comparer is Locale locale) return locale.IndexOf(source, value);

        for (int i = 0; i < source.Length - value.Length + 1; i++)
        {
            var span = source.AsSpan(i, value.Length);
            if (comparer.Compare(span.ToString(), value) == 0) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given value in the given source string,
    /// or -1 if it was not found. If the value is empty, then the returned value is cero.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int LastIndexOf(this string source, string value, IComparer<string> comparer)
    {
        source = source.ThrowWhenNull();
        value = value.ThrowWhenNull();
        comparer = comparer.ThrowWhenNull();

        if (value.Length == 0) return 0;
        if (value.Length > source.Length) return -1;

        if (comparer is Locale locale) return locale.IndexOf(source, value);

        for (int i = source.Length - value.Length; i >= 0; i--)
        {
            var span = source.AsSpan(i, value.Length);
            if (comparer.Compare(span.ToString(), value) == 0) return i;
        }
        return -1;
    }
}
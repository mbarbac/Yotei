namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given source string is null or empty after being trimmed, if such
    /// has been requested (by default). Otherwise, returns either the original string or the
    /// trimmed one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim = true)
    {
        if (source is null) return null;

        if (trim) // Only if requested...
        {
            var span = source.AsSpan().Trim();
            if (span.Length == 0) return null;
            if (span.Length == source.Length) return source;

            source = span.ToString();
        }

        return source.Length == 0 ? null : source;
    }

    /// <summary>
    /// Throws an <see cref="EmptyException"/> is the given source string is null or empty after
    /// being trimmed if such is requested (by default). Otherwise, returns either the original
    /// or the trimmed string.
    /// <br/> If no trim is requested, spaces-only strings are considered valid ones.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static string NotNullNotEmpty(
        this string? source,
        bool trim = true,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        description = description.NullWhenEmpty() ?? nameof(description);

        // Null ones throw by default...
        source = source.ThrowWhenNull(description);

        // Removing head and tail spaces only if requested...
        if (trim) source = source.NullWhenEmpty(trim);

        // Finishing...
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
    // int IndexOf(char)
    // int IndexOf(char, StringComparison)

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

    // ----------------------------------------------------
    // int IndexOf(char)
    // int IndexOf(char, StringComparison)

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

    // ----------------------------------------------------
    // int IndexOf(string)
    // int IndexOf(string, StringComparison)

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
        return source.AsSpan().IndexOf(value, caseSensitive);
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
        return source.AsSpan().IndexOf(value, comparer);
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
        return source.AsSpan().IndexOf(value, comparer);
    }

    // ----------------------------------------------------
    // int LastIndexOf(string)
    // int LastIndexOf(string, StringComparison)

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
        return source.AsSpan().LastIndexOf(value, caseSensitive);
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
        return source.AsSpan().LastIndexOf(value, comparer);
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
        return source.AsSpan().LastIndexOf(value, comparer);
    }

    // ----------------------------------------------------
    // bool Contains(char)
    // bool Contains(char, StringComparison)
    // bool Contains(char, IEqualityComparer<char>?)

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

    // ----------------------------------------------------
    // bool Contains(string)
    // bool Contains(string, StringComparison)

    /// <summary>
    /// Determines if the given source contains the given value, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static bool Contains(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();
        return source.AsSpan().Contains(value, caseSensitive);
    }

    /// <summary>
    /// Determines if the given source contains the given value, or not.
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
    /// Determines if the given source contains the given value, or not.
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
    /// Removes from the given source the first ocurrence of the given value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Remove(this string source, string value)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        var r = source.AsSpan().Remove(value, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().Remove(value, caseSensitive, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().Remove(value, comparer, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().Remove(value, comparer, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().Remove(value, comparison, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveLast(value, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveLast(value, caseSensitive, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveLast(value, comparer, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveLast(value, comparer, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveLast(value, comparison, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveAll(value, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveAll(value, caseSensitive, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveAll(value, comparer, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveAll(value, comparer, out var removed);
        return removed ? r.ToString() : source;
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

        var r = source.AsSpan().RemoveAll(value, comparison, out var removed);
        return removed ? r.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static string Remove(this string source, char c)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().Remove(c, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string Remove(this string source, char c, bool caseSensitive)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().Remove(c, caseSensitive, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(this string source, char c, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().Remove(c, comparer, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string Remove(this string source, char c, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().Remove(c, comparer, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the first ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(this string source, char c, StringComparison comparison)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().Remove(c, comparison, out var removed);
        return removed ? r.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char c)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveLast(c, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char c, bool caseSensitive)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveLast(c, caseSensitive, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char c, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveLast(c, comparer, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char c, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveLast(c, comparer, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source the last ocurrence of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveLast(this string source, char c, StringComparison comparison)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveLast(c, comparison, out var removed);
        return removed ? r.ToString() : source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char c)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveAll(c, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char c, bool caseSensitive)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveAll(c, caseSensitive, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char c, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveAll(c, comparer, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char c, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveAll(c, comparer, out var removed);
        return removed ? r.ToString() : source;
    }

    /// <summary>
    /// Removes from the given source all ocurrences of the given char.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char c, StringComparison comparison)
    {
        source.ThrowWhenNull();

        var r = source.AsSpan().RemoveAll(c, comparison, out var removed);
        return removed ? r.ToString() : source;
    }
}
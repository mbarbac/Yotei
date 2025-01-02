namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class StringExtensions
{
    // <summary>
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
        if (source is null || source.Length == 0)
            throw new ArgumentException($"{description} is null or empty.");

        return source;
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

        var r = source.AsSpan().Remove(value.AsSpan(), out var removed);
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

        var r = source.AsSpan().Remove(value.AsSpan(), caseSensitive, out var removed);
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

        var r = source.AsSpan().Remove(value.AsSpan(), comparer, out var removed);
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

        var r = source.AsSpan().Remove(value.AsSpan(), comparer, out var removed);
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

        var r = source.AsSpan().Remove(value.AsSpan(), comparison, out var removed);
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

        var r = source.AsSpan().RemoveLast(value.AsSpan(), out var removed);
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

        var r = source.AsSpan().RemoveLast(value.AsSpan(), caseSensitive, out var removed);
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

        var r = source.AsSpan().RemoveLast(value.AsSpan(), comparer, out var removed);
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

        var r = source.AsSpan().RemoveLast(value.AsSpan(), comparer, out var removed);
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

        var r = source.AsSpan().RemoveLast(value.AsSpan(), comparison, out var removed);
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

        var r = source.AsSpan().RemoveAll(value.AsSpan(), out var removed);
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

        var r = source.AsSpan().RemoveAll(value.AsSpan(), caseSensitive, out var removed);
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

        var r = source.AsSpan().RemoveAll(value.AsSpan(), comparer, out var removed);
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

        var r = source.AsSpan().RemoveAll(value.AsSpan(), comparer, out var removed);
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

        var r = source.AsSpan().RemoveAll(value.AsSpan(), comparison, out var removed);
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string with the given value removed at the end of the source. If the value
    /// is not found, or it is not found at the end, the original one is returned instead.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveEnd(this string source, string value)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.EndsWith(value))
        {
            var len = source.Length - value.Length;
            source = source[..len];
        }
        return source;
    }

    /// <summary>
    /// Returns a string with the given value removed at the end of the source. If the value
    /// is not found, or it is not found at the end, the original one is returned instead.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public static string RemoveEnd(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.EndsWith(value, caseSensitive))
        {
            var len = source.Length - value.Length;
            source = source[..len];
        }
        return source;
    }

    static bool EndsWith(this string source, string value, bool caseSensitive)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.Length == 0) return false;
        if (value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        var index = source.IndexOf(value, caseSensitive);
        if (index < 0) return false;
        if (index < (source.Length - value.Length)) return false;
        return true;
    }

    /// <summary>
    /// Returns a string with the given value removed at the end of the source. If the value
    /// is not found, or it is not found at the end, the original one is returned instead.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveEnd(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.EndsWith(value, comparer))
        {
            var len = source.Length - value.Length;
            source = source[..len];
        }
        return source;
    }

    static bool EndsWith(this string source, string value, IEqualityComparer<char> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.Length == 0) return false;
        if (value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return false;
        if (index < (source.Length - value.Length)) return false;
        return true;
    }

    /// <summary>
    /// Returns a string with the given value removed at the end of the source. If the value
    /// is not found, or it is not found at the end, the original one is returned instead.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static string RemoveEnd(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.EndsWith(value, comparer))
        {
            var len = source.Length - value.Length;
            source = source[..len];
        }
        return source;
    }

    static bool EndsWith(this string source, string value, IEqualityComparer<string> comparer)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.Length == 0) return false;
        if (value.Length == 0) return false;
        if (value.Length > source.Length) return false;

        var index = source.IndexOf(value, comparer);
        if (index < 0) return false;
        if (index < (source.Length - value.Length)) return false;
        return true;
    }

    /// <summary>
    /// Returns a string with the given value removed at the end of the source. If the value
    /// is not found, or it is not found at the end, the original one is returned instead.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string RemoveEnd(this string source, string value, StringComparison comparison)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.EndsWith(value, comparison))
        {
            var len = source.Length - value.Length;
            source = source[..len];
        }
        return source;
    }
}
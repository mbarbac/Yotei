namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given string is null or empty, or the original value instead. By
    /// default, the given value is trimmed before validating.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? value, bool trim = true)
    {
        if (value != null)
        {
            if (trim)
            {
                var span = value.AsSpan().Trim();
                if (span.Length == 0) return null;
                if (span.Length == value.Length) return value;

                value = span.ToString();
            }

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        return value;
    }

    /// <summary>
    /// Validates that the given string value is not null and not empty. By default, the given
    /// value is trimmed before validating.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="EmptyException"></exception>
    public static string NotNullNotEmpty(
        this string? value,
        bool trim = true,
        [CallerArgumentExpression(nameof(value))] string? name = default)
    {
        name = name.NullWhenEmpty(trim) ?? nameof(value);

        value = value.ThrowIfNull();

        if ((value = value.NullWhenEmpty(trim)) == null)
            throw new EmptyException($"'{name}' cannot be empty.");

        return value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given string contains any character in the given array.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string source, IEnumerable<char> array)
    {
        source = source.ThrowIfNull();
        array = array.ThrowIfNull();

        foreach (var c in array) if (source.Contains(c)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given string contains any character in the given array, using the given
    /// comparison value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> array, StringComparison comparison)
    {
        source = source.ThrowIfNull();
        array = array.ThrowIfNull();

        foreach (var c in array) if (source.Contains(c, comparison)) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given string contains any character in the given array, using the given
    /// comparer.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this string source, IEnumerable<char> array, IEqualityComparer<char> comparer)
    {
        source = source.ThrowIfNull();
        array = array.ThrowIfNull();
        comparer = comparer.ThrowIfNull();

        foreach (var c in array) if (source.Contains(c, comparer)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new string where the original diacritics have been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string value)
    {
        value = value.ThrowIfNull();

        var temp = value.Normalize(NormalizationForm.FormD);
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
    /// Removes the given <paramref name="remove"/> string from the source one, returning the
    /// new string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="remove"></param>
    /// <returns></returns>
    public static string Remove(this string source, string remove) => source.Remove(remove, out _);

    /// <summary>
    /// Removes the given <paramref name="remove"/> string from the source one, returning the
    /// new string. The out <paramref name="removed"/> determines if it has been found and
    /// removed, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="remove"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, string remove, out bool removed)
    {
        removed = false;

        source = source.ThrowIfNull(); if (source.Length == 0) return source;
        remove = remove.ThrowIfNull(); if (remove.Length == 0) return source;

        var index = source.IndexOf(remove);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, remove.Length);
    }

    /// <summary>
    /// Removes the given <paramref name="remove"/> string from the source one, using the given
    /// comparison to find it, returning the new string. The out <paramref name="removed"/>
    /// determines if it has been found and removed, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="remove"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static string Remove(
        this string source, string remove, StringComparison comparison)
        => source.Remove(remove, comparison);

    /// <summary>
    /// Removes the given <paramref name="remove"/> string from the source one, using the given
    /// comparison to find it, returning the new string. The out <paramref name="removed"/>
    /// determines if it has been found and removed, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="remove"></param>
    /// <param name="comparison"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static string Remove(this string source, string remove, StringComparison comparison, out bool removed)
    {
        removed = false;

        source = source.ThrowIfNull(); if (source.Length == 0) return source;
        remove = remove.ThrowIfNull(); if (remove.Length == 0) return source;

        var index = source.IndexOf(remove, comparison);
        if (index < 0) return source;

        removed = true;
        return source.Remove(index, remove.Length);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Wraps the given string with the given head and tail characters, provided it was not null
    /// and not empty after trimming, if such is requested.
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
            if (source.Length > 0) source = $"{head}{source}{tail}";
        }

        return source;
    }

    /// <summary>
    /// <summary>
    /// Wraps the given string with the given character, provided it was not null and not empty
    /// after trimming, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(
        this string? source, char ch, bool trim = true) => source.Wrap(ch, ch, trim);

    /// <summary>
    /// Unwraps the given string from the given head and tail characters, provided that they
    /// appear paired after trimming the source string, if requested. Returns null if that
    /// source string was null, or an unwrapped string if possible, which might be an empty
    /// one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? Unwrap(
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

    /// <summary>
    ///  Unwraps the given string from the given character, provided that it appears paired
    /// as the head and tail one after trimming the source string, if requested. Returns null
    /// if that source string was null, or an unwrapped string if possible, which might be an
    /// empty one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? Unwrap(
        this string? source, char ch, bool trim = true, bool recursive = true)
        => source.Unwrap(ch, ch, trim, recursive);
}
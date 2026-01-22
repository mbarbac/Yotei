namespace Yotei.Tools;

// Note: cannot use extension block when 'CallerArgumentExpression' is used.

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns <see langword="null"/> if the source string is null, or if it is empty after trimmed,
    /// if such is requested. Otherwise, returns either the original string or the trimmed one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim) => trim switch
    {
        true => string.IsNullOrWhiteSpace(source) ? null : source.Trim(),
        false => string.IsNullOrEmpty(source) ? null : source,
    };

    /// <summary>
    /// Trims the source string, if requested, and then throws a <see cref="ArgumentNullException"/>
    /// if the result is <see langword="null"/>, or a <see cref="EmptyException"/> is it is an empty
    /// one. Otherwise, returns either the original string or the trimmed one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? source, bool trim,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        if (string.IsNullOrWhiteSpace(description)) description = nameof(source);

        ArgumentNullException.ThrowIfNull(source, description);

        source = source.NullWhenEmpty(trim);
        if (source is null || source.Length == 0) throw new EmptyException(description);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new string where its diacritics marks have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        source = source!.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in source)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
        }
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Wraps the given source string with the given head and tail characters, provided it is not
    /// null and not empty. If trimming is requested, the source string is trimmed before validating
    /// it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(this string? source, char head, char tail, bool trim)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(head);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tail);

        if (source is not null)
        {
            var span = source.AsSpan();
            if (trim) span = span.Trim();

            if (span.Length > 0 &&
                span[0] != head &&
                span[^1] != tail)
                return $"{head}{span}{tail}";

            if (span.Length != source.Length) return span.ToString();
        }
        return source;
    }

    /// <summary>
    /// Wraps the given source string with the given character, provided it is not null and not empty.
    /// If trimming is requested, the source string is trimmed before validating it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(this string? source, char c, bool trim) => source.Wrap(c, c, trim);

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source string the given head and tail characters, provided they both
    /// appear paired. If trimming is requested, the source string is trimmed before validating it.
    /// If recursive is requested, then the method is iterated while needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? Unwrap(
        this string? source, char head, char tail, bool trim, bool recursive = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(head);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tail);

        if (source is not null)
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
            if (span.Length != source.Length) return span.ToString();
        }
        return source;
    }

    /// <summary>
    /// Removes from the given source string the given character, provided it appears both as the
    /// head and tail character of that string. If trimming is requested, the source string is trimmed
    /// before validating it. If recursive is requested, then the method is iterated while needed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? Unwrap(
        this string? source, char c, bool trim, bool recursive = true)
        => source.Unwrap(c, c, trim, recursive);
}
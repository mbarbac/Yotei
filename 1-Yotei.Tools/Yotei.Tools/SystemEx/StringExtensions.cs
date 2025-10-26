namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns '<c>null</c>' if the given source string is null or empty after trimming it if
    /// such is requested. Otherwise, returns either the original or the trimmed string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim) => trim switch
    {
        true => string.IsNullOrWhiteSpace(source) ? null : source.Trim(),
        false => string.IsNullOrEmpty(source) ? null : source
    };

    /// <summary>
    /// Throws an <see cref="EmptyException"/> if the given source string is null or empty after
    /// trimming it if such is requested. Returns either the original or the trimmed string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static string NotNullNotEmpty(
        this string? source,
        bool trim,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        if (string.IsNullOrWhiteSpace(description)) description = nameof(source);

        source = source.ThrowWhenNull();
        source = source.NullWhenEmpty(trim);
        if (source is null || source.Length == 0) throw new EmptyException(description);

        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new string where the original diacritics marks have been removed.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string source)
    {
        source.ThrowWhenNull();
        source = source!.Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var c in source)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
        }
        return sb.ToString();
    }

    // ------------------------------------------------

    /// <summary>
    /// Returns a string with the original value wrapped with the head and tail characters,
    /// provided that value is not null and not empty. If trimming is requested, then the 
    /// value is trimmed before validating it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(this string? source, char head, char tail, bool trim)
    {
        if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
        if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

        if (source is not null)
        {
            if (trim) source = source.Trim();
            var span = source.AsSpan();

            if (span.Length > 0 &&
                span[0] != head &&
                span[^1] != tail)
                source = $"{head}{source}{tail}";
        }

        return source;
    }

    /// <summary>
    /// Returns a string with the original value wrapped with the given character, provided
    /// that value is not null and not empty. If trimming is requested, then the value is
    /// trimmed before validating it.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="c"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? Wrap(this string? source, char c, bool trim) => source.Wrap(c, c, trim);

    // ------------------------------------------------

    /// <summary>
    /// Returns a string where the given head and tail characters have been removed from the
    /// original value, provided they are paired. If trimming is requested, the value is then
    /// trimmed before and after the removal. By default, unwrapping is perfomed recursively
    /// unless otherwise requested.
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
        if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
        if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

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
            if (trim) span = span.Trim();

            if (span.Length != source.Length) source = span.ToString();
        }

        return source;
    }

    /// <summary>
    /// Returns a string where the given character is removed from the head and tail of the
    /// original value, provided it appeard paired. If trimming is requested, the value is then
    /// trimmed before and after the removal. By default, unwrapping is perfomed recursively
    /// unless otherwise requested.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="trim"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static string? Unwrap(
        this string? source, char c, bool trim, bool recursive = true)
        => source.Unwrap(c, c, trim, recursive);
}
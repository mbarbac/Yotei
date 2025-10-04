namespace Yotei.Tools;

// ========================================================
public static partial class StringExtensions
{
    /// <summary>
    /// Throws an <see cref="EmptyException"/> if the given source string is null or empty. The
    /// the source is trimmed before validating if requested. Returns either the original string
    /// or the trimmed one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// Need to be placed outside extension block for CallerArgumentExpression to work.
    public static string NotNullNotEmpty(
        this string? source,
        bool trim,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        description = description.NullWhenEmpty(true) ?? nameof(source);

        source.ThrowWhenNull();
        source = source.NullWhenEmpty(trim);
        if (source is null || source.Length == 0) throw new EmptyException(description);

        return source;
    }
}

// ========================================================
public static partial class StringExtensions
{
    extension(string? source)
    {
        // ------------------------------------------------

        /// <summary>
        /// Returns null if this source string is null or empty, or the resulting string after
        /// trimming it, if such is requested.
        /// </summary>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? NullWhenEmpty(bool trim)
        {
            return trim switch
            {
                true => string.IsNullOrWhiteSpace(source) ? null : source.Trim(),
                false => string.IsNullOrEmpty(source) ? null : source
            };
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a new string where the original diacritics marks have been removed.
        /// </summary>
        /// <returns></returns>
        public string RemoveDiacritics()
        {
            source.ThrowWhenNull();
            source = source!.Normalize(NormalizationForm.FormD);

            var sb = StringBuilder.Pool.Rent();
            foreach (var c in source)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return StringBuilder.Pool.Return(sb);
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns a string with the original value wrapped with the head and tail characters,
        /// provided that value is not null and not empty. If trimming is requested, the that
        /// value is trimmed before validating it.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? Wrap(char head, char tail, bool trim)
        {
            if (head <= 0) throw new ArgumentException("Invalid head.").WithData(head);
            if (tail <= 0) throw new ArgumentException("Invalid tail.").WithData(tail);

            if (source is not null)
            {
                if (trim) source = source.Trim();

                if (source.Length > 0 &&
                    source[0] != head &&
                    source[^1] != tail)
                    source = $"{head}{source}{tail}";
            }
            return source;
        }

        /// <summary>
        /// Returns a string with the original value wrapped with the given character, provided
        /// that value is not null and not empty. If trimming is requested, the that value is
        /// trimmed before validating it.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public string? Wrap(char c, bool trim) => source.Wrap(c, c, trim);

        // ------------------------------------------------

        /// <summary>
        /// Returns a string where the given head and tail characters have been removed from the
        /// original value, provided they are paired. If trimming is requested, the value is then
        /// trimmed before and after the removal. By default, unwrapping is perfomed recursively
        /// unless otherwise requested.
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="trim"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public string? Unwrap(char head, char tail, bool trim, bool recursive = true)
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
                source = span.ToString();
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
        public string? Unwrap(
            char c, bool trim, bool recursive = true) => source.Unwrap(c, c, trim, recursive);
    }
}
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

            var sb = new StringBuilder();
            foreach (var c in source)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class StringExtensions
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

    /// <summary>
    /// Returns null if this source string is null or empty, or the resulting string after
    /// trimming it, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public static string? NullWhenEmpty(this string? source, bool trim)
    {
        return trim switch
        {
            true => string.IsNullOrWhiteSpace(source) ? null : source!.Trim(),
            false => string.IsNullOrEmpty(source) ? null : source
        };
    }
}
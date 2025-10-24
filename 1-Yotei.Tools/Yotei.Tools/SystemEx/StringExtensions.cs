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
}
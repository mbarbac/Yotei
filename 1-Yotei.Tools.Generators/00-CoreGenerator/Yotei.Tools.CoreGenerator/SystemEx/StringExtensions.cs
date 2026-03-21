namespace Yotei.Tools.CoreGenerator;

// Note: cannot use extension block when 'CallerArgumentExpression' is used.

// ========================================================
internal static class StringExtensions
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
        true => string.IsNullOrWhiteSpace(source) ? null : source!.Trim(),
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
}
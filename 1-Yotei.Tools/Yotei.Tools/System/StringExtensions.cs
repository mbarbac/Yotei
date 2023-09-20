namespace Yotei.Tools;

// ========================================================
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given source string is null, or empty after trimming it if such was
    /// requested. Otherwise, returns the original string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim = true)
    {
        if (source is not null && trim)
        {
            var span = source.AsSpan().Trim();

            if (span.Length == 0) return null;
            if (span.Length == source.Length) return source;
            return span.IsWhiteSpace() ? null : span.ToString();
        }
        return source;
    }

    /// <summary>
    /// Throws an appropriate exception if the given source string is null, or empty after
    /// trimming it if such was requested. Otherwise, returns the validated string.
    /// <para>If the 'description' argument was omitted, then the source name is used.</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? source,
        bool trim = true,
        [CallerArgumentExpression(nameof(source))] string? description = default)
    {
        description ??= nameof(source);

        ArgumentNullException.ThrowIfNull(source);
        if (trim)
        {
            source = source.NullWhenEmpty();
            if (source is null) throw new EmptyException($"'{description}' cannot be empty.");
        }
        return source;
    }
}
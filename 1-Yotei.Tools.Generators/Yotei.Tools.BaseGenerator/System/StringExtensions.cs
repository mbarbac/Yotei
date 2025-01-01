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
}
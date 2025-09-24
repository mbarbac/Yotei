namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Extensions for <see cref="String"/> instances.
/// TODO: Use C# 14 static extension methods to extend 'String' capabilities.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the given source string is null or empty, or the resulting string after
    /// trimming it, if such is requested.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim)
    {
        if (source is null) return null;

        if (trim)
        {
            var span = source.AsSpan().Trim();
            if (span.Length == 0) return null;
            if (span.Length == source.Length) return source;

            source = span.ToString();
        }

        return source.Length == 0 ? null : source;
    }

    /// <summary>
    /// Throws an <see cref="EmptyException"/> if the given source string is null or empty. If
    /// requested, the source is trimmed before validating. Returns either the original string or
    /// the trimmed one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? source,
        bool trim,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        description = description.NullWhenEmpty(true) ?? nameof(description);

        source.ThrowWhenNull();
        source = source.NullWhenEmpty(trim);
        if (source is null || source.Length == 0) throw new EmptyException(description);

        return source;
    }
}
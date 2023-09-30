namespace Yotei.Tools.Generators;

// ========================================================
internal static class StringExtensions
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
    /// <br/> If the 'description' argument was omitted, then the source name is used.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="description"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? source,
        string description,
        bool trim = true)
    {
        description = description.NullWhenEmpty() ?? nameof(source);

        if (source == null) throw new ArgumentNullException($"'{description}' is null.");
        if (trim)
        {
            source = source.NullWhenEmpty();
            if (source is null) throw new EmptyException($"'{description}' cannot be empty.");
        }
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new span where the last ocurrence of the given value has been removed from the
    /// original source one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ReadOnlySpan<char> RemoveLast(
        this ReadOnlySpan<char> source,
        ReadOnlySpan<char> value)
    {
        if (source.Length == 0) return ReadOnlySpan<char>.Empty;
        if (value.Length == 0) return source;

        var index = source.LastIndexOf(value);
        if (index >= 0)
        {
            var len = source.Length - value.Length;
            return source.Slice(0, len);
        }

        return source;
    }
}
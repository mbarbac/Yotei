namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class StringExtensions
{
    /// <summary>
    /// Returns null if the given string is null or empty, after trimming it if such is requested.
    /// Otherwise returns the given string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? value, bool trim = true)
    {
        if (value is not null && trim)
        {
            var span = value.AsSpan().Trim();

            if (span.Length == 0) return null;
            if (span.Length == value.Length) return value;
            value = span.IsWhiteSpace() ? null : span.ToString();
        }
        return value;
    }

    /// <summary>
    /// Throws an exception if the given string is null or empty, after trimming it if such is
    /// requested. Otherwise returns the validated string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="description"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? value,
        string? description,
        bool trim = true)
    {
        description = description.NullWhenEmpty() ?? nameof(value);

        value.ThrowWhenNull(nameof(value));
        if (trim)
        {
            value = value.NullWhenEmpty(trim);
            if (value is null) throw new ArgumentException($"Value is empty.", description);
        }
        return value!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new span where the last ocurrence of the given value has been removed from the
    /// original one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ReadOnlySpan<char> RemoveLast(
        this ReadOnlySpan<char> source,
        ReadOnlySpan<char> value)
    {
        if (source.Length == 0) return [];
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
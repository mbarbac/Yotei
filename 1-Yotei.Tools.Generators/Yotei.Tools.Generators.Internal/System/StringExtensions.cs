namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class StringExtensions
{
    /// <summary>
    /// Returns null if the given source string is null or empty, after trimming it if such is
    /// requested. Otherwise, returns the resulting string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NullWhenEmpty(this string? source, bool trim = true)
    {
        if (source is null) return null;

        if (trim)
        {
            var span = source.AsSpan().Trim();

            if (span.Length == 0) return null;
            if (span.Length == source.Length) return source;
            source = span.ToString();
        }
        return string.IsNullOrWhiteSpace(source) ? null : source;
    }

    /// <summary>
    /// Throws an exception if the given source string is null or empty, after trimming it if
    /// such is requested. Otherwise, returns the validated string.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="trim"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullNotEmpty(
        this string? source,
        bool trim = true,
        [CallerArgumentExpression(nameof(source))] string? description = null)
    {
        description = description.NullWhenEmpty() ?? nameof(description);

        source = source.NullWhenEmpty(trim);
        source = source.ThrowWhenNull(description);
        return source!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new string with all the ocurrences of the given char removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static string RemoveAll(this string source, char ch)
    {
        source.ThrowWhenNull();

        if (source.Length == 0) return source;
        if (!source.Contains(ch)) return source;

        var cs = new List<char>();
        foreach (var c in source) if (c != ch) cs.Add(c);
        return new string(cs.ToArray());
    }
}
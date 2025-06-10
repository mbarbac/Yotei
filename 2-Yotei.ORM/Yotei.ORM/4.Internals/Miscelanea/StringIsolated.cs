namespace Yotei.ORM.Internals;

// ========================================================
public static class StringIsolated
{
    /// <summary>
    /// The default set of characters that act as separators for isolated strings.
    /// </summary>
    public static readonly ImmutableArray<char> SEPARATORS =
        " ()[]{}<>\"^'`´\\|!&@#=+-*/%"
        .ToImmutableArray();

    /// <summary>
    /// Returns the index of the first isolated ocurrence of the given value in the given text,
    /// starting at the given index, or -1 if not found. Isolation is tested using the given sets
    /// of characters as head and tail separators, which by default are null to use a default
    /// set of separators.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="heads"></param>
    /// <param name="tails"></param>
    /// <returns></returns>
    public static int FindIsolated(
        this string value, string? source, int ini,
        IEnumerable<char>? heads = null,
        IEnumerable<char>? tails = null)
    {
        var comparison = StringComparison.CurrentCulture;
        return value.FindIsolated(source, ini, comparison, heads, tails);
    }

    /// <summary>
    /// Returns the index of the first isolated ocurrence of the given value in the given text,
    /// starting at the given index, or -1 if not found. Isolation is tested using the given sets
    /// of characters as head and tail separators, which by default are null to use a default
    /// set of separators.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="comparison"></param>
    /// <param name="heads"></param>
    /// <param name="tails"></param>
    /// <returns></returns>
    public static int FindIsolated(
        this string value, string? source, int ini, StringComparison comparison,
        IEnumerable<char>? heads = null,
        IEnumerable<char>? tails = null)
    {
        value.NotNullNotEmpty();
        if (ini < 0) throw new ArgumentException("Initial position cannot be negative.").WithData(ini);

        if (source is null || source.Length == 0) return -1;

        heads ??= SEPARATORS;
        tails ??= SEPARATORS;

        var pos = source.IndexOf(value, ini, comparison);
        if (pos < 0) return -1;

        if (pos > 0)
        {
            var c = source[pos - 1];
            if (!heads.Contains(c)) return -1;
        }

        var len = pos + value.Length;
        if (len < source.Length)
        {
            var c = source[len];
            if (!tails.Contains(c)) return -1;
        }

        return pos;
    }
}
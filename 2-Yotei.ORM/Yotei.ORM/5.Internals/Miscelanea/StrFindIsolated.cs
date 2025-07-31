namespace Yotei.ORM.Internals;

// ========================================================
public static class StrFindIsolated
{
    /// <summary>
    /// The default collection of separators for isolated values.
    /// </summary>
    public static readonly ImmutableArray<char> SEPARATORS =
        " ()[]{}<>\"'`´\\|!&@#=+-*/%^"
        .ToImmutableArray();

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source string,
    /// starting at the given index, or -1 if not found. Isolation is tested using both the given
    /// head and tails separators that, if null, use default ones, and whether or not the value
    /// appears at the start or end of the source sequence.
    /// </summary>
    public static int FindIsolated(
        this string source, string value, int ini,
        bool sensitive,
        IEnumerable<char>? heads = null, IEnumerable<char>? tails = null)
    {
        var comparison = sensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return FindIsolated(source, value, ini, comparison, heads, tails);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source string,
    /// starting at the given index, or -1 if not found. Isolation is tested using both the given
    /// head and tails separators that, if null, use default ones, and whether or not the value
    /// appears at the start or end of the source sequence.
    /// </summary>
    public static int FindIsolated(
        this string source, string value, int ini,
        StringComparison comparison,
        IEnumerable<char>? heads = null, IEnumerable<char>? tails = null)
    {
        source = source.ThrowWhenNull();
        value = value.NotNullNotEmpty();
        heads ??= SEPARATORS;
        tails ??= SEPARATORS;

        if (source.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var pos = source.IndexOf(value, ini, comparison);
        if (pos >= 0)
        {
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
        }

        return pos;
    }
}
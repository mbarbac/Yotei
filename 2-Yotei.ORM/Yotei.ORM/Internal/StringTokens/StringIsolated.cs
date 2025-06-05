namespace Yotei.ORM.Internal;

// ========================================================
public static class StringIsolated
{
    /// <summary>
    /// The default set of characters that act as head or tail separators for isolated string
    /// values.
    /// </summary>
    public static readonly ImmutableArray<char> SEPARATORS
        = " ()[]{}<>\"^'`´\\|!&@#=+-*/%"
        .ToImmutableArray();

    /// <summary>
    /// Returns the index of the first isolated ocurrence of the given value in the given text,
    /// starting at the given index, or -1 if not found, using the given heads and tails sets of
    /// characters as separators, and the current culture's string comparison.
    /// <br/> If heads or tails are null then a default set is used.
    /// <br/> Note that head and tail characters are always case sensitive.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="heads"></param>
    /// <param name="tails"></param>
    /// <returns></returns>
    public static int FindIsolated(
        this string value, string source, int ini,
        ImmutableArray<char>? heads = null!,
        ImmutableArray<char>? tails = null!)
    {
        var comparison = StringComparison.CurrentCulture;
        return value.FindIsolated(source, ini, comparison, heads, tails);
    }

    /// <summary>
    /// Returns the index of the first isolated ocurrence of the given value in the given text,
    /// starting at the given index, or -1 if not found, using the given heads and tails sets of
    /// characters as separators, and the given string comparison.
    /// <br/> If heads or tails are null then a default set is used.
    /// <br/> Note that head and tail characters are always case sensitive.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="comparison"></param>
    /// <param name="heads"></param>
    /// <param name="tails"></param>
    /// <returns></returns>
    public static int FindIsolated(
        this string value, string source, int ini, StringComparison comparison,
        ImmutableArray<char>? heads = null!,
        ImmutableArray<char>? tails = null!)
    {
        heads ??= SEPARATORS;
        tails ??= SEPARATORS;

        value.NotNullNotEmpty();
        if (ini < 0) throw new ArgumentException("Initial position cannot be negative.").WithData(ini);

        if (source is null || source.Length == 0) return -1;

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
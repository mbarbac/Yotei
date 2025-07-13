using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Internals;

// ========================================================
public static class StrFindIsolated
{
    /// <summary>
    /// The default collection of character separators for isolated values.
    /// </summary>
    public static readonly ImmutableArray<char> SEPARATORS =
        " ()[]{}<>\"^'`´\\|!&@#=+-*/%"
        .ToImmutableArray();

    /// <summary>
    /// Returns the index of the first ocurrence of the isolated value in the given source,
    /// starting at the given index, or -1 if not found. Isolation is tested using the given
    /// heads and tails collection of separators that, if null, use a default one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ini"></param>
    /// <param name="comparison"></param>
    /// <param name="heads"></param>
    /// <param name="tails"></param>
    /// <returns></returns>
    public static int FindIsolated(
        this string source, string value, int ini,
        StringComparison comparison,
        IEnumerable<char>? heads = null,
        IEnumerable<char>? tails = null)
    {
        source.ThrowWhenNull();
        value.NotNullNotEmpty();

        if (source.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

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

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the isolated value in the given source,
    /// starting at the given index, or -1 if not found. Isolation is tested using the given
    /// heads and tails collection of separators that, if null, use a default one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ini"></param>
    /// <param name="sensitive"></param>
    /// <param name="heads"></param>
    /// <param name="tails"></param>
    /// <returns></returns>
    public static int FindIsolated(
        this string source, string value, int ini,
        bool sensitive,
        IEnumerable<char>? heads = null,
        IEnumerable<char>? tails = null)
    {
        var comparison = sensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return source.FindIsolated(value, ini, comparison, heads, tails);
    }
}
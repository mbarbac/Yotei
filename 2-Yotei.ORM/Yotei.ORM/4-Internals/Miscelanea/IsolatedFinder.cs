using StrSpan = System.ReadOnlySpan<char>;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Used to find isolated literals in a given source string. Literals are considered isolated
/// when they are wrapped by valid head and tail separators, they including being at the head
/// or tail of the given source string.
/// </summary>
public record IsolatedFinder
{
    public static readonly ImmutableArray<char> SEPARATORS = [.. " ()[]{}<>\"'`´\\|!&@#=+-*/%^"];

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public IsolatedFinder() { }

    /// <summary>
    /// The collection of characters serving as head separators.
    /// </summary>
    public ImmutableArray<char> Heads { get; init => field = value.ThrowWhenNull(); }
    = SEPARATORS;

    /// <summary>
    /// The collection of characters serving as tail separators.
    /// </summary>
    public ImmutableArray<char> Tails { get; init => field = value.ThrowWhenNull(); }
    = SEPARATORS;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, starting
    /// at the given index, or -1 if not found, using a default char comparer.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value)
        => Find(source, ini, value, char.CharComparer());

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, starting
    /// at the given index, or -1 if not found, using the given case comparer mode.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value, bool ignoreCase)
        => Find(source, ini, value, char.CharComparer(ignoreCase));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, starting
    /// at the given index, or -1 if not found, using the given comparison.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value, StringComparison comparison)
        => Find(source, ini, value, char.CharComparer(comparison));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, starting
    /// at the given index, or -1 if not found, using the given comparer.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value, IEqualityComparer<string> comparer)
        => Find(source, ini, value, char.CharComparer(comparer));

    /// <summary>
    /// Returns the index of the first ocurrence of the given value in the given source, starting
    /// at the given index, or -1 if not found, using the given comparer.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value, IEqualityComparer<char> comparer)
    {
        source = source[ini..];

        if (source.Length == 0) return -1;
        if (value.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var pos = source.IndexOf(value, comparer);
        if (pos < 0) return -1;

        // Validating heads...
        if (pos > 0)
        {
            var c = source[pos - 1];
            if (!Heads.Contains(c, comparer)) return -1;
        }

        // Validating tails...
        var len = pos + value.Length;
        if (len < source.Length)
        {
            var c = source[len];
            if (!Tails.Contains(c, comparer)) return -1;
        }

        // Adjusting to the slicing...
        return pos + ini;
    }
}
using StrSpan = System.ReadOnlySpan<char>;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Used to find isolated literals in a given source string.
/// <br/> Literals are considered to be isolated when they are wrapped between valid head and
/// tails separators, even if they appear at the beginning or end of the source string.
/// </summary>
public record IsolatedFinder
{
    /// <summary>
    /// The default collection of char separators.
    /// </summary>
    public static readonly ImmutableArray<char> SEPARATORS = [.. " ()[]{}<>\"'`´\\|!&@#=+-*/%^"];

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public IsolatedFinder() { }

    /// <summary>
    /// The collection of head separator characters. If an empty ones, then only literals that
    /// appear at the beginning of a souce string may qualify as isolated ones (provided they
    /// have a valid tail separator).
    /// </summary>
    public IEnumerable<char> Heads
    {
        get => _Heads;
        set => _Heads = value.ThrowWhenNull() is ImmutableArray<char> temp ? temp : [.. value];
    }
    ImmutableArray<char> _Heads = SEPARATORS;

    /// <summary>
    /// The collection of tail separator characters. If an empty ones, then only literals that
    /// appear at the end of a souce string may qualify as isolated ones (provided they have a
    /// valid head separator).
    /// </summary>
    public IEnumerable<char> Tails
    {
        get => _Tails;
        set => _Tails = value.ThrowWhenNull() is ImmutableArray<char> temp ? temp : [.. value];
    }
    ImmutableArray<char> _Tails = SEPARATORS;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first isoltaed ocurrence of the given value in the given source
    /// one, starting at the given position. Returns -1 if an isolated value was not found.
    /// <br/> Empty sources or values will never match, this method returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value)
        => Find(source, ini, value, char.CharComparer());

    /// <summary>
    /// Returns the index of the first isoltaed ocurrence of the given value in the given source
    /// one, starting at the given position. Returns -1 if an isolated value was not found.
    /// <br/> Empty sources or values will never match, this method returns -1.
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
    /// Returns the index of the first isoltaed ocurrence of the given value in the given source
    /// one, starting at the given position. Returns -1 if an isolated value was not found.
    /// <br/> Empty sources or values will never match, this method returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public int Find(StrSpan source, int ini, StrSpan value, IEqualityComparer<char> comparer)
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
            if (!_Heads.Contains(c, comparer)) return -1;
        }

        // Validating tails...
        var len = pos + value.Length;
        if (len < source.Length)
        {
            var c = source[len];
            if (!_Tails.Contains(c, comparer)) return -1;
        }

        // Adjusting to the slicing...
        return pos + ini;
    }

    /// <summary>
    /// Returns the index of the first isoltaed ocurrence of the given value in the given source
    /// one, starting at the given position. Returns -1 if an isolated value was not found.
    /// <br/> Empty sources or values will never match, this method returns -1.
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
    /// Returns the index of the first isoltaed ocurrence of the given value in the given source
    /// one, starting at the given position. Returns -1 if an isolated value was not found.
    /// <br/> Empty sources or values will never match, this method returns -1.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ini"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public int Find(
        StrSpan source, int ini, StrSpan value, StringComparison comparison)
        => Find(source, ini, value, char.CharComparer(comparison));
}
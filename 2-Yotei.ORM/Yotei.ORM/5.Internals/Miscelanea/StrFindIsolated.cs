namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Used to find isolated instances of a given value in a given source string.
/// <br/> Isolation is achieved when the value is wrapped between valid head and tail separators,
/// being valid ones when it appears at the start or end of the given source.
/// </summary>
public class StrFindIsolated
{
    /// <summary>
    /// The default collection of char separators for isolated values.
    /// </summary>
    public static readonly ImmutableArray<char>
        SEPARATORS = " ()[]{}<>\"'`´\\|!&@#=+-*/%^".ToImmutableArray();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public StrFindIsolated() { }

    /// <summary>
    /// The collection of head separator characters. If an empty one, then only values that appear
    /// at the start of the given source may qualify as isolated ones.
    /// </summary>
    public IEnumerable<char> Heads
    {
        get => _Heads;
        set
        {
            value.ThrowWhenNull();
            _Heads = value.ToImmutableArray();
        }
    }
    ImmutableArray<char> _Heads = SEPARATORS;

    /// <summary>
    /// The collection of tails separator characters. If an empty one, then only values that appear
    /// at the end of the given source may qualify as isolated ones.
    /// </summary>
    public IEnumerable<char> Tails
    {
        get => _Tails;
        set
        {
            value.ThrowWhenNull();
            _Tails = value.ToImmutableArray();
        }
    }
    ImmutableArray<char> _Tails = SEPARATORS;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first ocurrence of the given isolated value in the given source
    /// string, starting at the given position, or -1 if such is not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ini"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public int Find(string source, string value, int ini, bool sensitive)
    {
        var comp = sensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return Find(source, value, ini, comp);
    }

    /// <summary>
    /// Returns the index of the first ocurrence of the given isolated value in the given source
    /// string, starting at the given position, or -1 if such is not found.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="ini"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public int Find(string source, string value, int ini, StringComparison comparison)
    {
        source.ThrowWhenNull();
        value.ThrowWhenNull();

        if (source.Length == 0) return -1;
        if (value.Length > source.Length) return -1;

        var pos = source.IndexOf(value, ini, comparison);
        if (pos >= 0)
        {
            if (pos > 0)
            {
                var c = source[pos - 1];
                if (!Heads.Contains(c)) return -1;
            }

            var len = pos + value.Length;
            if (len < source.Length)
            {
                var c = source[len];
                if (!Tails.Contains(c)) return -1;
            }
        }

        return pos;
    }
}
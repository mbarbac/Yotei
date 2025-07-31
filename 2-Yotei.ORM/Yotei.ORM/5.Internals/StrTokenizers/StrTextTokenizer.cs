namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenText"/> tokens.
/// </summary>
public record StrTextTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="sequence"></param>
    public StrTextTokenizer(string sequence) => Sequence = sequence;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="sequence"></param>
    public StrTextTokenizer(char sequence)
    {
        Sequence = Validate(sequence).ToString();

        static char Validate(char c) => c >= ' '
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Sequence}";

    // ----------------------------------------------------

    /// <summary>
    /// The not-null and not-empty sequence to extract.
    /// <br/> Spaces-only values are considered valid ones.
    /// </summary>
    public string Sequence
    {
        get => _Sequence;
        init => _Sequence = value.NotNullNotEmpty(trim: false);
    }
    string _Sequence = default!;

    /// <summary>
    /// Whether to keep the sequence as specified in this instance, or rather use the one found
    /// in the given source (by default).
    /// </summary>
    public bool KeepSequence { get; init; }

    /// <summary>
    /// If not null, the string sequence that if found preceding the requested one, prevents it
    /// from being tokenized. If <c>null</c>, then this property is ignored.
    /// </summary>
    public string? Escape
    {
        get => _Escape;
        init => _Escape = value?.NotNullNotEmpty(trim: false);
    }
    string? _Escape = null;

    /// <summary>
    /// Whether, if a escape sequence is found, keep it in the returned result, or rather not to
    /// keep it (by default).
    /// </summary>
    public bool KeepEscape { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a found token using the given value.
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(string sequence) => new StrTokenText(sequence);

    /// <summary>
    /// Determines if the found value is a valid one or not, taking into consideration how it
    /// appears at the given index in the given source (for instance, validating it is wrapped
    /// between appropriate separators, etc).
    /// </summary>
    /// <param name="index"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual bool IsValueAtIndex(int index, string source) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        // Trivial cases...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Main loop...
        string? xcape = Escape is null ? null : (Escape + Sequence);
        StrTokenChain.Builder builder = [];
        StringBuilder sb = new();

        IStrToken token;
        string str;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source.AsSpan(i);

            // Escape+Value...
            if (xcape is not null && span.StartsWith(xcape, Comparison))
            {
                sb.Append(KeepEscape
                    ? source.Substring(i, xcape.Length)
                    : source.Substring(i + Escape!.Length, Sequence.Length));

                i += xcape.Length; i--;
                continue;
            }

            // Value...
            if (span.StartsWith(Sequence, Comparison))
            {
                if (!IsValueAtIndex(i, source)) continue;

                if (sb.Length > 0)
                {
                    token = new StrTokenText(sb.ToString());
                    builder.Add(token);
                    sb.Clear();
                }

                str = KeepSequence ? Sequence : source.Substring(i, Sequence.Length);
                token = CreateToken(str);
                builder.Add(token);

                i += Sequence.Length; i--;
                continue;
            }

            // Others...
            sb.Append(source[i]);
        }

        // Finishing...
        if (sb.Length > 0)
        {
            if (sb.Length == source.Length) return new StrTokenText(source);

            token = new StrTokenText(sb.ToString());
            builder.Add(token);
            sb.Clear();
        }
        return builder.Count == 0 ? StrTokenText.Empty : builder.CreateInstance();
    }
}
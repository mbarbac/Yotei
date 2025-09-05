namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenText"/> tokens from a given source.
/// </summary>
public record StrTextTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public StrTextTokenizer(string target) => Target = target;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public StrTextTokenizer(char target)
    {
        Target = Validate(target).ToString();

        static char Validate(char c) => c >= ' '
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Target}";

    // ----------------------------------------------------

    /// <summary>
    /// The not-null and not-empty sequence to extract. Spaces-only sequences are considered
    /// valid ones.
    /// </summary>
    public string Target
    {
        get => _Target;
        init => _Target = value.NotNullNotEmpty(trim: false);
    }
    string _Target = default!;

    /// <summary>
    /// Whether to keep the target sequence as specified in this instance, or rather use the
    /// one found in the given source (by default).
    /// </summary>
    public bool KeepTarget { get; init; }

    /// <summary>
    /// If not null, the string sequence that, if found preceding the target one, prevents it
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
    /// Invoked to create a new token to carry the target that has been found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(string value) => new StrTokenText(value);

    /// <summary>
    /// Determines if a found target at the given index is valid, taking into consideration how
    /// it appears in the source string (ie: if it is wrapped between appropriate separators,
    /// etc).
    /// </summary>
    /// <param name="index"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual bool IsValidTargetAt(int index, string source) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        // Trivial cases...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Main loop...
        string? xcape = Escape is null ? null : (Escape + Target);
        StrTokenChain.Builder builder = [];
        StringBuilder temp = new();
        IStrToken token;
        string str;

        for (int i = 0; i < source.Length; i++)
        {
            var span = source.AsSpan(i);

            // Escape+Target...
            if (xcape is not null && span.StartsWith(xcape, Comparison))
            {
                temp.Append(KeepEscape
                    ? source.Substring(i, xcape.Length)
                    : source.Substring(i + Escape!.Length, Target.Length));

                i += xcape.Length; i--;
                continue;
            }

            // Target...
            if (span.StartsWith(Target, Comparison))
            {
                if (!IsValidTargetAt(i, source)) continue;

                if (temp.Length > 0)
                {
                    token = new StrTokenText(temp.ToString());
                    builder.Add(token);
                    temp.Clear();
                }

                str = KeepTarget ? Target : source.Substring(i, Target.Length);
                token = CreateToken(str);
                builder.Add(token);

                i += Target.Length; i--;
                continue;
            }

            // Others...
            temp.Append(source[i]);
        }

        // Remaining...
        if (temp.Length > 0)
        {
            if (temp.Length == source.Length) return new StrTokenText(source);

            token = new StrTokenText(temp.ToString());
            builder.Add(token);
            temp.Clear();
        }

        // Finishing...
        return builder.Count == 0 ? StrTokenText.Empty : builder.CreateInstance();
    }
}
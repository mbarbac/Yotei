namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenLiteral"/> tokens.
/// </summary>
[InheritWiths]
public partial class StrLiteralTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrLiteralTokenizer(string value) => Value = value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrLiteralTokenizer(char value)
    {
        Value = Validate(value).ToString();

        static char Validate(char c) => c >= 32
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrLiteralTokenizer(StrLiteralTokenizer source)
    {
        Comparison = source.Comparison;
        Value = source.Value;
        ValueFromSource = source.ValueFromSource;
        Escape = source.Escape;
        RemoveEscape = source.RemoveEscape;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Value}";

    /// <summary>
    /// The not-null and not-empty value to extract as literal tokens. Note that values that
    /// consist in spaces only are considered valid ones.
    /// </summary>
    [With]
    public string Value
    {
        get => _Value;
        init => _Value = value.NotNullNotEmpty(trim: false);
    }
    string _Value = default!;

    /// <summary>
    /// Whether to return the literal value from the one found in the source, or rather from
    /// the normalized one in this instance (by default).
    /// </summary>
    [With] public bool ValueFromSource { get; init; }

    /// <summary>
    /// If not null the string sequence that, if found preceding the requested value, prevents
    /// it from being tokenized. If <c>null</c> then this setting is ignored. If not, the value
    /// if trimmed before used and spaces only escape sequences are not valid.
    /// </summary>
    [With]
    public string? Escape
    {
        get => _Escape;
        init => _Escape = value?.NotNullNotEmpty(trim: true);
    }
    string? _Escape = null;

    /// <summary>
    /// When the escape sequence is found preceding a requested value, whether to leave it as
    /// a text element in the returned token chain (by default), or to remove it instead.
    /// </summary>
    [With] public bool RemoveEscape { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create an extracted token using the given identified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(string value) => new StrTokenLiteral(value);

    /// <summary>
    /// Invoked to validate that it is possible to extract the requested value found at the
    /// given index in the given source string, considering all characters in that source.
    /// For instance, keywords may need appropriate head and tail separators.
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

        // Preparing...
        string? xcape = Escape is null ? null : (Escape + Value);
        StrTokenChain.Builder builder = [];
        int last = 0;

        IStrToken token;
        string str;
        int len;

        // Main loop...
        for (int i = 0; i < source.Length; i++)
        {
            var span = source.AsSpan(i);

            // Escape+Value found...
            if (xcape is not null && span.StartsWith(xcape, Comparison))
            {
                len = i - last;
                str = len > 0 ? source.Substring(last, len) : string.Empty;

                str += RemoveEscape
                    ? source.Substring(i + Escape!.Length, Value.Length)
                    : source.Substring(i, xcape.Length);

                token = new StrTokenText(str);
                builder.Add(token);

                last = (i += xcape.Length);
                i--;
                continue;
            }

            // Value found...
            if (span.StartsWith(Value, Comparison) && IsValueAtIndex(i, source))
            {
                len = i - last; if (len > 0)
                {
                    str = source.Substring(last, len);
                    token = new StrTokenText(str);
                    builder.Add(token);
                }

                str = ValueFromSource ? source.Substring(i, Value.Length) : Value;
                token = CreateToken(str);
                builder.Add(token);

                last = (i += Value.Length);
                i--;
                continue;
            }
        }

        // Remaining characters...
        len = source.Length - last; if (len > 0)
        {
            str = source.Substring(last, len);
            token = new StrTokenText(str);
            builder.Add(token);
        }

        // Finishing...
        return builder.ToInstance();
    }
}
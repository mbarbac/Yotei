namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of extracting arbitrary <see cref="IStrTokenText"/> tokens.
/// <br/> Note that if 'reduce' is requested while tokenizing, adjacent text elements may get
/// combined in the final result.
/// </summary>
[InheritWiths]
public partial class StrTokenizerText : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenizerText(string value) => Value = value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrTokenizerText(char value)
    {
        Value = Validate(value).ToString();

        static char Validate(char c) => c >= ' '
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenizerText(StrTokenizerText source) : base(source)
    {
        Value = source.Value;
        ValueFromSource = source.ValueFromSource;
        Escape = source.Escape;
        RemoveEscape = source.RemoveEscape;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Value}";

    // ----------------------------------------------------

    /// <summary>
    /// The not-null and not-empty value to extract.
    /// <br/> Note that spaces-only values are considered as valid ones.
    /// </summary>
    [With]
    public string Value
    {
        get => _Value;
        init => _Value = value.NotNullNotEmpty(trim: false);
    }
    string _Value = default!;

    /// <summary>
    /// Whether to use the value as it appeared in the given source, or rather, by default,
    /// use the value as specified in this instance.
    /// </summary>
    [With] public bool ValueFromSource { get; init; }

    /// <summary>
    /// If not null the string sequence that, if found preceding the requested value, prevents
    /// it for being tokenized. If '<c>null</c>', by default, this property is ignored.
    /// </summary>
    [With]
    public string? Escape
    {
        get => _Escape;
        init => _Escape = value?.NotNullNotEmpty(trim: false);
    }
    string? _Escape = null;

    /// <summary>
    /// When the escape sequence is found preceding the requested value, whether to leave it as
    /// a text element in the returned token chain (by default), or rather to remove it instead.
    /// </summary>
    [With] public bool RemoveEscape { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a token for the found value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(string value) => new StrTokenText(value);

    /// <summary>
    /// Determines if the value is found at the given index in the given source. This method is
    /// invoked to validate that the value is found considering the context that sorrounds it.
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
        string? xcape = Escape is null ? null : (Escape + Value);
        StrTokenChain.Builder builder = [];
        int last = 0;

        IStrToken token;
        string str;
        int len;

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
        len = source.Length - last;
        if (len > 0)
        {
            str = source.Substring(last, len);
            token = new StrTokenText(str);
            builder.Add(token);
        }

        // Finishing...
        return builder.ToInstance();
    }
}
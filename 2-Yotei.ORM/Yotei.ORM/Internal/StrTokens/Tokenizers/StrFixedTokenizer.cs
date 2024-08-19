namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Extracts <see cref="IStrTokenFixed"/> tokens from a given source.
/// </summary>
[InheritWiths]
public partial class StrFixedTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrFixedTokenizer(string value) => Value = value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public StrFixedTokenizer(char value) => Value = Validate(value).ToString();

    static char Validate(char c) => c >= 32
        ? c
        : throw new ArgumentException("Invalid character.").WithData(c);

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected StrFixedTokenizer(StrFixedTokenizer source)
    {
        Comparison = source.Comparison;
        ReduceSource = source.ReduceSource;
        ReduceResult = source.ReduceResult;

        Value = source.Value;
        PreventSourceValue = source.PreventSourceValue;
        Escape = source.Escape;
        KeepEscape = source.KeepEscape;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{Value}]";

    /// <summary>
    /// The fixed value to extract from a given source.
    /// </summary>
    [With]
    public string Value
    {
        get => _Value;
        set => _Value = value.NotNullNotEmpty(trim: false);
    }
    string _Value = default!;

    /// <summary>
    /// If <c>true</c> then the value found in the source sequence is replaced by the one defined
    /// by this instance. The default <c>false</c> keeps the source sequence value.
    /// </summary>
    [With]
    public bool PreventSourceValue { get; set; }

    /// <summary>
    /// If not null, the sequence that if appears right after the value one, prevents it from
    /// being tokenized.
    /// </summary>
    [With]
    public string? Escape
    {
        get => _Escape;
        set => _Escape = value?.NotNullNotEmpty();
    }
    string? _Escape = null;

    /// <summary>
    /// If <c>true</c> then the escape sequence is kept in the tokenized result. The default
    /// <c>false</c> one removes it.
    /// </summary>
    [With]
    public bool KeepEscape { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate an appropriate token from a matching sequence.
    /// </summary>
    protected virtual IStrToken Generator(string value)
    {
        return new StrTokenFixed(value);
    }

    /// <summary>
    /// Provides an additional validation about if the given source, at the given index, contains
    /// a valid value sequence or not, in addition of the standard lexicografical comparison.
    /// </summary>
    protected virtual bool IsValueAtIndex(int index, string source) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        // Trivial case...
        if (source is null) return StrTokenText.Empty;

        // Preparing...
        string? xcape = Escape is null ? null : Escape + Value;
        StrTokenChain.Builder chain = [];
        int last = 0;

        IStrToken token;
        int len;
        string str;

        // Main loop...
        for (int i = 0; i < source.Length; i++)
        {
            var span = source.AsSpan(i);

            // Escape sequence...
            if (xcape is not null && span.StartsWith(xcape, Comparison))
            {
                if (KeepEscape) // We need to remove the escape sequence...
                {
                    len = i - last; if (len > 0)
                    {
                        str = source.Substring(last, len);
                        token = new StrTokenText(str);
                        chain.Add(token);
                    }

                    str = source.Substring(i, xcape.Length);
                    token = new StrTokenText(str);
                    chain.Add(token);

                    last = (i += xcape.Length);
                    i--;
                    continue;
                }
                else // Remove the escape sequence...
                {
                    len = i - last;
                    str = len > 0 ? source.Substring(last, len) : string.Empty;

                    str += source.Substring(i + Escape!.Length, Value.Length);
                    token = new StrTokenText(str);
                    chain.Add(token);

                    last = (i += xcape.Length);
                    i--;
                    continue;
                }
            }

            // Value sequence...
            if (span.StartsWith(Value, Comparison) && IsValueAtIndex(i, source))
            {
                len = i - last; if (len > 0)
                {
                    str = source.Substring(last, len);
                    token = new StrTokenText(str);
                    chain.Add(token);
                }

                str = PreventSourceValue ? Value : source.Substring(i, Value.Length);
                token = Generator(str);
                chain.Add(token);

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
            chain.Add(token);
        }

        // Finishing...
        return chain.ToInstance();
    }
}
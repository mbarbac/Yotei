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
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected StrFixedTokenizer(StrFixedTokenizer source)
    {
        Comparison = source.Comparison;
        ReduceSource = source.ReduceSource;
        ReduceResult = source.ReduceResult;

        Value = source.Value;
        UseSourceValue = source.UseSourceValue;
        Escape = source.Escape;
        RemoveEscape = source.RemoveEscape;
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
    /// If <c>true</c> then the value found in the source sequence is kept in the returned token.
    /// The default <c>false</c> one replaces it with the one specified in this instance.
    /// </summary>
    [With]
    public bool UseSourceValue { get; set; }

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
    /// If <c>true</c> then the escape sequence is removed from the tokenized result. The default
    /// <c>false</c> one keeps it.
    /// </summary>
    [With]
    public bool RemoveEscape { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate an appropriate token from a matching sequence.
    /// </summary>
    protected virtual IStrToken Generator(string value)
    {
        return new StrTokenFixed(value);
    }

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
                if (RemoveEscape) // We need to remove the escape sequence...
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
                else // Otherwise...
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
            }

            // Value sequence...
            if (span.StartsWith(Value, Comparison))
            {
                len = i - last; if (len > 0)
                {
                    str = source.Substring(last, len);
                    token = new StrTokenText(str);
                    chain.Add(token);
                }

                str = UseSourceValue ? source.Substring(i, Value.Length) : Value;
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
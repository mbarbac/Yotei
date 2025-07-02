namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenWrapped"/> tokens.
/// </summary>
[InheritWiths]
public partial class StrWrappedTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public StrWrappedTokenizer(string head, string tail)
    {
        Head = head;
        Tail = tail;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="separator"></param>
    public StrWrappedTokenizer(string separator) : this(separator, separator) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public StrWrappedTokenizer(char head, char tail)
    {
        Head = Validate(head).ToString();
        Tail = Validate(tail).ToString();

        static char Validate(char c) => c >= ' '
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="separator"></param>
    public StrWrappedTokenizer(char separator) : this(separator, separator) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrWrappedTokenizer(StrWrappedTokenizer source) : base(source)
    {
        Head = source.Head;
        Tail = source.Tail;
        WrappersFromSource = source.WrappersFromSource;
        Escape = source.Escape;
        RemoveEscape = source.RemoveEscape;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Head},{Tail}";

    // ----------------------------------------------------

    /// <summary>
    /// The not-null and not-empty head sequence.
    /// <br/> Spaces-only sequences are NOT considered valid ones.
    /// </summary>
    [With]
    public string Head
    {
        get => _Head;
        init => _Head = value.NotNullNotEmpty();
    }
    string _Head = default!;

    /// <summary>
    /// The not-null and not-empty head sequence.
    /// <br/> Spaces-only sequences are NOT considered valid ones.
    /// </summary>
    [With]
    public string Tail
    {
        get => _Tail;
        init => _Tail = value.NotNullNotEmpty();
    }
    string _Tail = default!;

    /// <summary>
    /// Whether to use the wrappers as they appeared in the given source, or rather, by default,
    /// use the ones specified in this instance.
    /// </summary>
    [With] public bool WrappersFromSource { get; init; }

    /// <summary>
    /// If not null the string sequence that, if found preceding the head or tail ones, prevents
    /// them for being tokenized. If '<c>null</c>', by default, this property is ignored.
    /// </summary>
    [With]
    public string? Escape
    {
        get => _Escape;
        init => _Escape = value?.NotNullNotEmpty(trim: false);
    }
    string? _Escape = null;

    /// <summary>
    /// When the escape sequence is found preceding the head or tail sequences, whether to leave
    /// them as text elements in the returned token chain (by default), or rather to remove them
    /// instead.
    /// </summary>
    [With] public bool RemoveEscape { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a token for the found head and tail sequences, and payload.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="payload"></param>
    /// <param name="tail"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(
        string head, IStrToken payload, string tail) => new StrTokenWrapped(head, payload, tail);

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        var same = string.Compare(Head, Tail, Comparison) == 0;

        return same
            ? ExtractSameWrappers(source)
            : ExtractDifferentWrappers(source);
    }

    // ----------------------------------------------------

    class XHead(string value) : StrTokenLiteral(value) { }

    class HeadTokenizer : StrLiteralTokenizer
    {
        public HeadTokenizer(StrWrappedTokenizer master) : base(master.Head)
        {
            Comparison = master.Comparison;
            ValueFromSource = true; // Enforced, we'll treat it as needed...
            Escape = master.Escape;
            RemoveEscape = master.RemoveEscape;
        }

        protected override IStrToken CreateToken(string value) => new XHead(value);
    }

    /// <summary>
    /// Invoked when the head and tail sequences are the same one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    IStrToken ExtractSameWrappers(string source)
    {
        // Trivial case...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Preparing...
        var tokenizer = new HeadTokenizer(this);
        var token = tokenizer.Tokenize(source);

        // Case: chained result...
        if (token is StrTokenChain range)
        {
            var builder = new StrTokenChain.Builder(range);

            // Finding paired sequences...
            var ini = -1;
            for (int i = 0; i < builder.Count; i++)
            {
                var item = builder[i];

                if (item is XHead xhead)
                {
                    if (ini < 0) ini = i;
                    else
                    {
                        var len = i - ini - 1;
                        var head = WrappersFromSource ? xhead.Payload : Head;

                        item = len switch
                        {
                            0 => StrTokenText.Empty,
                            _ => new StrTokenChain(builder.ToList().GetRange(ini + 1, len))
                        };
                        item = CreateToken(head, item, head);

                        builder.RemoveRange(ini, len + 2);
                        builder.Insert(ini, item);
                        ini = i = -1;
                    }
                }
            }

            // Removing orphan ones...
            for (int i = 0; i < builder.Count; i++)
            {
                var item = builder[i];
                if (item is XHead xhead) builder[i] = new StrTokenText(xhead.Payload);
            }

            // Restoring...
            token = builder.CreateInstance();
        }

        // Case: single result...
        else
        {
            if (token is XHead xhead) token = new StrTokenText(xhead.Payload);
        }

        // Finishing...
        return token;
    }

    // ----------------------------------------------------

    class XTail(string value) : StrTokenLiteral(value) { }

    class TailTokenizer : StrLiteralTokenizer
    {
        public TailTokenizer(StrWrappedTokenizer master) : base(master.Tail)
        {
            Comparison = master.Comparison;
            ValueFromSource = true; // Enforced, we'll treat it as needed...
            Escape = master.Escape;
            RemoveEscape = master.RemoveEscape;
        }

        protected override IStrToken CreateToken(string value) => new XTail(value);
    }

    /// <summary>
    /// Invoked when the head and tail sequences are different ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    IStrToken ExtractDifferentWrappers(string source)
    {
        // Trivial case...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Preparing...
        var htokenizer = new HeadTokenizer(this);
        var token = htokenizer.Tokenize(source);

        var ttokenizer = new TailTokenizer(this);
        token = ttokenizer.Tokenize(token);

        // Case: chained result...
        if (token is StrTokenChain range)
        {
            var builder = new StrTokenChain.Builder(range);

            // Finding paired sequences...
            var ini = -1;
            for (int i = 0; i < builder.Count; i++)
            {
                var item = builder[i];

                if (item is XHead)
                {
                    ini = i;
                    continue;
                }

                if (item is XTail xtail && ini >= 0)
                {
                    var len = i - ini - 1;
                    var xhead = (XHead)builder[ini];

                    var head = WrappersFromSource ? xhead.Payload : Head;
                    var tail = WrappersFromSource ? xtail.Payload : Tail;

                    item = len switch
                    {
                        0 => StrTokenText.Empty,
                        _ => new StrTokenChain(builder.ToList().GetRange(ini + 1, len))
                    };
                    item = CreateToken(head, item, tail);

                    builder.RemoveRange(ini, len + 2);
                    builder.Insert(ini, item);
                    ini = i = -1;
                }
            }

            // Removing orphan ones...
            for (int i = 0; i < builder.Count; i++)
            {
                var item = builder[i];

                if (item is XHead xhead) builder[i] = new StrTokenText(xhead.Payload);
                if (item is XTail xtail) builder[i] = new StrTokenText(xtail.Payload);
            }

            // Restoring....
            token = builder.CreateInstance();
        }

        // Case: single result...
        else
        {
            if (token is XHead xhead) token = new StrTokenText(xhead.Payload);
            if (token is XTail xtail) token = new StrTokenText(xtail.Payload);
        }

        // Finishing...
        return token;
    }
}
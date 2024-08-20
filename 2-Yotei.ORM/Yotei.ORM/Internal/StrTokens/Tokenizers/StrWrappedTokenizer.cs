namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Extracts <see cref="IStrTokenWrapped"/> tokens from a given source.
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
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public StrWrappedTokenizer(char head, char tail)
    {
        Head = Validate(head).ToString();
        Tail = Validate(tail).ToString();
    }

    static char Validate(char c) => c >= 32
        ? c
        : throw new ArgumentException("Invalid character.").WithData(c);

    /// <summary>
    /// Initializes a new instance when the head and tail sequences are the same one.
    /// </summary>
    /// <param name="wrapper"></param>
    public StrWrappedTokenizer(string wrapper) : this(wrapper, wrapper) { }

    /// <summary>
    /// Initializes a new instance when the head and tail sequences are the same one.
    /// </summary>
    /// <param name="wrapper"></param>
    public StrWrappedTokenizer(char wrapper) : this(wrapper, wrapper) { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source"></param>
    protected StrWrappedTokenizer(StrWrappedTokenizer source)
    {
        Comparison = source.Comparison;
        ReduceSource = source.ReduceSource;
        ReduceResult = source.ReduceResult;

        Head = source.Head;
        Tail = source.Tail;
        PreventSourceWrappers = source.PreventSourceWrappers;
        Escape = source.Escape;
        KeepEscape = source.KeepEscape;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{Head},{Tail}]";

    /// <summary>
    /// The head sequence.
    /// </summary>
    [With]
    public string Head
    {
        get => _Head;
        set => _Head = value.NotNullNotEmpty(trim: false);
    }
    string _Head = default!;

    /// <summary>
    /// The tail sequence.
    /// </summary>
    [With]
    public string Tail
    {
        get => _Tail;
        set => _Tail = value.NotNullNotEmpty(trim: false);
    }
    string _Tail = default!;

    /// <summary>
    /// If <c>true</c> then the head and value found in the source sequence are replaced by the
    /// ones defined by this instance. The default <c>false</c> keeps the source sequence head
    /// and tail values.
    /// </summary>
    [With]
    public bool PreventSourceWrappers { get; set; }

    /// <summary>
    /// If not null, the sequence that if appears right after the head or tail ones, prevents
    /// them from being tokenized.
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
    protected virtual IStrToken Generator(string head, IStrToken value, string tail)
    {
        return new StrTokenWrapped(head, value, tail);
    }

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        var same = string.Compare(Head, Tail, Comparison) == 0;
        return same
            ? ExtractSame(source)
            : ExtractDifferent(source);
    }

    // ----------------------------------------------------

    class XHead(string value) : StrTokenFixed(value) { }
    class HeadTokenizer : StrFixedTokenizer
    {
        public HeadTokenizer(StrWrappedTokenizer master) : base(master.Head)
        {
            Comparison = master.Comparison;
            ReduceSource = master.ReduceSource;
            ReduceResult = master.ReduceResult;

            PreventSourceValue = false; // Enforced, later we'll treat it...
            Escape = master.Escape;
            KeepEscape = master.KeepEscape;
        }
        protected override IStrToken Generator(string value) => new XHead(value);
    }

    /// <inheritdoc/>
    IStrToken ExtractSame(string source)
    {
        // Trivial case...
        if (source is null) return StrTokenText.Empty;

        // Preparing...
        var tokenizer = new HeadTokenizer(this);
        var token = tokenizer.Tokenize(source);

        // Chained result...
        if (token is IStrTokenChain)
        {
            var chain = new StrTokenChain.Builder(token);

            // Finding paired sequences...
            var ini = -1;
            for (int i = 0; i < chain.Count; i++)
            {
                var item = chain[i];

                if (item is XHead xhead)
                {
                    if (ini < 0) ini = i;
                    else
                    {
                        var len = i - ini - 1;
                        var head = PreventSourceWrappers ? Head : xhead.Payload;

                        item = len switch
                        {
                            0 => StrTokenText.Empty,
                            _ => new StrTokenChain(chain.ToList(ini + 1, len))
                        };
                        item = Generator(head, item, head);

                        chain.RemoveRange(ini, len + 2);
                        chain.Insert(ini, item);
                        ini = i = -1;
                    }
                }
            }

            // Removing orphan ones...
            for (int i = 0; i < chain.Count; i++)
            {
                var item = chain[i];
                if (item is XHead xhead) chain[i] = new StrTokenText(xhead.Payload);
            }

            // Removing not needed escape elements...
            if (!KeepEscape && Escape is not null)
            {
                for (int i = 0; i < chain.Count; i++)
                {
                    var item = chain[i];
                    if (item is StrTokenText temp && string.Compare(Escape, temp.Payload, Comparison) == 0)
                    {
                        chain.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Restoring...
            token = chain.ToInstance();
        }

        // Single result...
        else
        {
            if (token is XHead xhead) token = new StrTokenText(xhead.Payload);
        }

        // Finishing...
        return token;
    }

    // ----------------------------------------------------

    class XTail(string value) : StrTokenFixed(value) { }
    class TailTokenizer : StrFixedTokenizer
    {
        public TailTokenizer(StrWrappedTokenizer master) : base(master.Tail)
        {
            Comparison = master.Comparison;
            ReduceSource = master.ReduceSource;
            ReduceResult = master.ReduceResult;

            PreventSourceValue = false; // Enforced, later we'll treat it...
            Escape = master.Escape;
            KeepEscape = master.KeepEscape;
        }
        protected override IStrToken Generator(string value) => new XTail(value);
    }

    /// <inheritdoc/>
    IStrToken ExtractDifferent(string source)
    {
        // Trivial case...
        if (source is null) return StrTokenText.Empty;

        // Preparing...
        var htokenizer = new HeadTokenizer(this);
        var token = htokenizer.Tokenize(source);

        var ttokenizer = new TailTokenizer(this);
        token = ttokenizer.Tokenize(token);

        // Chained result...
        if (token is IStrTokenChain)
        {
            var chain = new StrTokenChain.Builder(token);

            // Finding paired sequences...
            var ini = -1;
            for (int i = 0; i < chain.Count; i++)
            {
                var item = chain[i];

                if (item is XHead)
                {
                    ini = i;
                    continue;
                }
                if (item is XTail xtail && ini >= 0)
                {
                    var len = i - ini - 1;
                    var xhead = (XHead)chain[ini];
                    var head = PreventSourceWrappers ? Head : xhead.Payload;
                    var tail = PreventSourceWrappers ? Tail : xtail.Payload;

                    item = len switch
                    {
                        0 => StrTokenText.Empty,
                        _ => new StrTokenChain(chain.ToList(ini + 1, len))
                    };
                    item = Generator(head, item, tail);

                    chain.RemoveRange(ini, len + 2);
                    chain.Insert(ini, item);
                    ini = i = -1;
                }
            }

            //  Removing orphan ones...
            for (int i = 0; i < chain.Count; i++)
            {
                var item = chain[i];

                if (item is XHead xhead) chain[i] = new StrTokenText(xhead.Payload);
                if (item is XTail xtail) chain[i] = new StrTokenText(xtail.Payload);
            }

            // Removing not needed escape elements...
            if (!KeepEscape && Escape is not null)
            {
                for (int i = 0; i < chain.Count; i++)
                {
                    var item = chain[i];
                    if (item is StrTokenText temp && string.Compare(Escape, temp.Payload, Comparison) == 0)
                    {
                        chain.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Restoring...
            token = chain.ToInstance();
        }

        // Single result...
        else
        {
            if (token is XHead xhead) token = new StrTokenText(xhead.Payload);
            if (token is XTail xtail) token = new StrTokenText(xtail.Payload);
        }

        // Finishing...
        return token;
    }
}
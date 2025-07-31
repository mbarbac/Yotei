using System.Diagnostics.Contracts;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenWrapped"/> tokens.
/// </summary>
public record StrWrapTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public StrWrapTokenizer(string head, string tail)
    {
        Head = head;
        Tail = tail;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="terminator"></param>
    public StrWrapTokenizer(string terminator) : this(terminator, terminator) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    public StrWrapTokenizer(char head, char tail)
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
    /// <param name="terminator"></param>
    public StrWrapTokenizer(char terminator) : this(terminator, terminator) { }

    /// <inheritdoc/>
    public override string ToString() => $"{Head},{Tail}";

    // ----------------------------------------------------

    /// <summary>
    /// The not-null and not-empty head sequence.
    /// <br/> Spaces-only values are considered valid ones.
    /// </summary>
    public string Head
    {
        get => _Head;
        init => _Head = value.NotNullNotEmpty(trim: false);
    }
    string _Head = default!;

    /// <summary>
    /// The not-null and not-empty tail sequence.
    /// <br/> Spaces-only values are considered valid ones.
    /// </summary>
    public string Tail
    {
        get => _Tail;
        init => _Tail = value.NotNullNotEmpty(trim: false);
    }
    string _Tail = default!;

    /// <summary>
    /// Whether to keep the head and tail terminators as specified in this instance, or rather use
    /// the ones found in the given source (by default).
    /// </summary>
    public bool KeepTerminators { get; init; }

    /// <summary>
    /// If not null, the string sequence that if found preceding the requested head or tail ones,
    /// prevent them from being tokenized. If <c>null</c>, then this property is ignored.
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
    /// Invoked to create a found token using the given terminators and payload.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(
        string head, IStrToken payload, string tail) => new StrTokenWrapped(head, payload, tail);

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        var same = string.Compare(Head, Tail, Comparison) == 0;

        return same
            ? ExtractWithSameTerminators(source)
            : ExtractWithDistinctTerminators(source);
    }

    // ----------------------------------------------------

    class XHead(string sequence) : StrTokenLiteral(sequence) { }
    record HeadTokenizer : StrLiteralTokenizer
    {
        public HeadTokenizer(StrWrapTokenizer master) : base(master.Head)
        {
            KeepSequence = false;
            Escape = master.Escape;
            KeepEscape = master.KeepEscape;
            Comparison = master.Comparison;
        }
        protected override IStrToken CreateToken(string head) => new XHead(head);
    }

    /// <summary>
    /// Invoked to extract the requested tokens when the terminators are the same.
    /// </summary>
    IStrToken ExtractWithSameTerminators(string source)
    {
        // Trivial cases...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Preparing...
        var tokenizer = new HeadTokenizer(this);
        var token = tokenizer.Tokenize(source, reduce: true);

        // Case: chained result...
        if (token is StrTokenChain chain)
        {
            var builder = chain.CreateBuilder();
            var changed = false;

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
                        var head = KeepTerminators ? Head : xhead.Payload;

                        item = len switch
                        {
                            0 => StrTokenText.Empty,
                            _ => new StrTokenChain(builder.ToList(ini + 1, len))
                        };
                        item = CreateToken(head, item, head);

                        builder.RemoveRange(ini, len + 2);
                        builder.Insert(ini, item);
                        ini = i = -1;
                        changed = true;
                    }
                }
            }

            // Removing orphan ones...
            for (int i = 0; i < builder.Count; i++)
            {
                var item = builder[i];
                if (item is XHead xhead) { builder[i] = new StrTokenText(xhead.Payload); changed = true; }
            }

            // Restoring...
            if (changed) token = builder.CreateInstance();
        }

        // Case: other results...
        else
        {
            if (token is XHead xhead) token = new StrTokenText(xhead.Payload);
        }

        // Finishing...
        return token;
    }

    // ----------------------------------------------------

    class XTail(string sequence) : StrTokenLiteral(sequence) { }
    record TailTokenizer : StrLiteralTokenizer
    {
        public TailTokenizer(StrWrapTokenizer master) : base(master.Tail)
        {
            KeepSequence = false;
            Escape = master.Escape;
            KeepEscape = master.KeepEscape;
            Comparison = master.Comparison;
        }
        protected override IStrToken CreateToken(string tail) => new XTail(tail);
    }

    /// <summary>
    /// Invoked to extract the requested tokens when the terminators are distinct.
    /// </summary>
    IStrToken ExtractWithDistinctTerminators(string source)
    {
        // Trivial cases...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Preparing...
        var htokenizer = new HeadTokenizer(this);
        var token = htokenizer.Tokenize(source, reduce: true);

        var ttokenizer = new TailTokenizer(this);
        token = ttokenizer.Tokenize(token, reduce: true);

        // Case: chained result...
        if (token is StrTokenChain chain)
        {
            var builder = chain.CreateBuilder();
            var changed = false;

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

                    var head = KeepTerminators ? Head : xhead.Payload;
                    var tail = KeepTerminators ? Tail : xtail.Payload;

                    item = len switch
                    {
                        0 => StrTokenText.Empty,
                        _ => new StrTokenChain(builder.ToList(ini + 1, len))
                    };
                    item = CreateToken(head, item, tail);

                    builder.RemoveRange(ini, len + 2);
                    builder.Insert(ini, item);
                    ini = i = -1;
                    changed = true;
                }
            }

            // Removing orphan ones...
            for (int i = 0; i < builder.Count; i++)
            {
                var item = builder[i];
                if (item is XHead xhead) { builder[i] = new StrTokenText(xhead.Payload); changed = true; }
                if (item is XTail xtail) { builder[i] = new StrTokenText(xtail.Payload); changed = true; }
            }

            // Restoring...
            if (changed) token = builder.CreateInstance();
        }

        // Case: other results...
        else
        {
            if (token is XHead xhead) token = new StrTokenText(xhead.Payload);
            if (token is XTail xtail) token = new StrTokenText(xtail.Payload);
        }

        // Finishing...
        return token;
    }
}
namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Extracts keyword tokens from a given source.
/// </summary>
[InheritWiths]
public partial class StrKeywordTokenizer : StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="keyword"></param>
    public StrKeywordTokenizer(string keyword) => Keyword = keyword;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    protected StrKeywordTokenizer(StrKeywordTokenizer source)
    {
        Comparison = source.Comparison;
        ReduceSource = source.ReduceSource;
        ReduceResult = source.ReduceResult;

        Keyword = source.Keyword;
        PreventSourceKeyword = source.PreventSourceKeyword;
        Heads = source.Heads;
        Tails = source.Tails;

        Escape = source.Escape;
        KeepEscape = source.KeepEscape;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{Keyword}]";

    /// <summary>
    /// The keyword to extract from a given source.
    /// </summary>
    [With]
    public string Keyword
    {
        get => _Keyword;
        set => _Keyword = value.NotNullNotEmpty(trim: false);
    }
    string _Keyword = default!;

    /// <summary>
    /// If <c>true</c> then the keyword found in the source sequence is replaced by the one
    /// defined by this instance. The default <c>false</c> keeps the source sequence one.
    /// </summary>
    [With]
    public bool PreventSourceKeyword { get; set; }

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

    /// <summary>
    /// The default set of characters that act as head separators for the keyword, in addition to
    /// it appearing at the start of a sequence.
    /// </summary>
    public char[] Heads
    {
        get => _Heads;
        set => _Heads = Validate(value);
    }
    char[] _Heads = Separators;

    /// <summary>
    /// The default set of characters that act as tail separators for the keyword, in addition to
    /// it appearing at the end of a sequence.
    /// </summary>
    public char[] Tails
    {
        get => _Tails;
        set => _Tails = Validate(value);
    }
    char[] _Tails = Separators;

    // ----------------------------------------------------

    /// <summary>
    /// he default set of characters that act as head and tail separators for the keyword, in
    /// addition to it appearing at the start or end of a sequence.
    /// </summary>
    public static readonly char[] Separators = " ()[]{}.,:;-+*/^!?=&%´'\"".ToCharArray();

    static char[] Validate(char[] array)
    {
        array.ThrowWhenNull();
        for (int i = 0; i < array.Length; i++)
        {
            char c = array[i];
            if (c < 32) throw new ArgumentException("Invalid character.").WithData(c);
        }
        return array;
    }

    // ----------------------------------------------------

    class InnerTokenizer : StrFixedTokenizer
    {
        public InnerTokenizer(StrKeywordTokenizer master) : base(master.Keyword)
        {
            Master = master;
            Comparison = master.Comparison;
            ReduceSource = master.ReduceSource;
            ReduceResult = master.ReduceResult;

            PreventSourceValue = master.PreventSourceKeyword;
            Escape = master.Escape;
            KeepEscape = master.KeepEscape;
        }
        StrKeywordTokenizer Master;

        protected override bool IsValueAtIndex(int index, string source)
        {
            if (index > 0)
            {
                var c = source[index - 1];
                if (!Master.Heads.Contains(c)) return false;
            }

            var len = index + Master.Keyword.Length;
            if (len < source.Length)
            {
                var c = source[len];
                if (!Master.Tails.Contains(c)) return false;
            }

            return true;
        }
    }

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        if (source is null) return StrTokenText.Empty;

        var tokenizer = new InnerTokenizer(this);
        var token = tokenizer.Tokenize(source);
        return token;
    }
}
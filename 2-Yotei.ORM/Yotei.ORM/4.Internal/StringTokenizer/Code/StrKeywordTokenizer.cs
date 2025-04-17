namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the ability of extracting <see cref="StrTokenKeyword"/> tokens.
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
    /// Initializes a new instance.
    /// </summary>
    /// <param name="keyword"></param>
    public StrKeywordTokenizer(char keyword)
    {
        Keyword = Validate(keyword).ToString();

        static char Validate(char c) => c >= 32
            ? c
            : throw new ArgumentException("Invalid character.").WithData(c);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrKeywordTokenizer(StrKeywordTokenizer source)
    {
        Comparison = source.Comparison;
        Keyword = source.Keyword;
        KeywordFromSource = source.KeywordFromSource;
        Escape = source.Escape;
        RemoveEscape = source.RemoveEscape;
        Heads = source.Heads;
        Tails = source.Tails;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Keyword}";

    /// <summary>
    /// The not-null and not-empty keyword to extract. Keywords are trimmed before used.
    /// </summary>
    [With]
    public string Keyword
    {
        get => _Keyword;
        init => _Keyword = value.NotNullNotEmpty(trim: true);
    }
    string _Keyword = default!;

    /// <summary>
    /// Whether to return the keyword from the value found in the source, or rather from
    /// the normalized one in this instance (by default).
    /// </summary>
    [With] public bool KeywordFromSource { get; init; }

    /// <summary>
    /// If not null the string sequence that, if found preceding the requested keyword, prevents
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
    /// When the escape sequence is found preceding a requested keyword, whether to leave it as
    /// a text element in the returned token chain (by default), or to remove it instead.
    /// </summary>
    [With] public bool RemoveEscape { get; init; }

    /// <summary>
    /// The default collection of chars that act as head and tail separators for keywords that
    /// are not isolated ones.
    /// </summary>
    public static readonly ImmutableArray<char> Separators = " ()[]{}<>=!|&?^'`´/*\\\""
        .ToImmutableArray();

    static ImmutableArray<char> Validate(ImmutableArray<char> array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            char c = array[i];
            if (c < ' ') throw new ArgumentException("Invalid character.").WithData(c);
        }
        return array;
    }

    /// <summary>
    /// The set of characters that act as head separators for non isolated keywords.
    /// </summary>
    [With]
    public ImmutableArray<char> Heads
    {
        get => _Heads;
        init => _Heads = Validate(value);
    }
    ImmutableArray<char> _Heads = Separators;

    /// <summary>
    /// The set of characters that act as tail separators for non isolated keywords.
    /// </summary>
    [With]
    public ImmutableArray<char> Tails
    {
        get => _Tails;
        init => _Tails = Validate(value);
    }
    ImmutableArray<char> _Tails = Separators;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create an extracted token using the given identified keyword.
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    protected virtual IStrToken CreateToken(string keyword) => new StrTokenKeyword(keyword);

    // ----------------------------------------------------

    /// <summary>
    /// Used to extract literal-alike tokens that will be transformed into keywords.
    /// </summary>
    class InnerTokenizer : StrLiteralTokenizer
    {
        readonly StrKeywordTokenizer Master;

        public InnerTokenizer(StrKeywordTokenizer master) : base(master.Keyword)
        {
            Master = master;
            Comparison = master.Comparison;
            ValueFromSource = master.KeywordFromSource;
            Escape = master.Escape;
            RemoveEscape = master.RemoveEscape;
        }

        protected override IStrToken CreateToken(string value) => Master.CreateToken(value);

        protected override bool IsValueAtIndex(int index, string source)
        {
            if (index > 0 && Master.Heads.Length > 0)
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

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IStrToken Extract(string source)
    {
        // Trivial cases...
        if (source is null || source.Length == 0) return StrTokenText.Empty;

        // Tokenizing...
        var tokenizer = new InnerTokenizer(this);
        var token = tokenizer.Tokenize(source);

        // Finishing...
        return token;
    }
}
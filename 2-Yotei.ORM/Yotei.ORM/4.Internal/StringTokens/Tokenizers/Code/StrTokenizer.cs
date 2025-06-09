namespace Yotei.ORM.Internal.Code;

// ========================================================
/// <inheritdoc cref="IStrTokenizer"/>
[InheritWiths]
public abstract partial class StrTokenizer : IStrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public StrTokenizer() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected StrTokenizer(StrTokenizer source)
    {
        Comparison = source.Comparison;
        OnlyText = source.OnlyText;
    }

    /// <inheritdoc/>
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;

    /// <inheritdoc/>
    public bool OnlyText { get; init; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Tokenize(string source, bool reduce = true)
    {
        var token = new StrTokenText(source);
        return Tokenize(token, reduce);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Tokenize(IStrToken source, bool reduce = true)
    {
        source.ThrowWhenNull();

        // It may happen that the things to search for are split between adjacent text elements...
        if (source is IStrTokenChain chain) source = chain.ReduceText();

        // We may need to intercept...
        if (OnlyText && source is not IStrTokenText) return source;

        // Otherwise, tokenizing...
        source = source.TokenizeWith(Extract);

        // But after tokenization we only reduce if requested...
        if (reduce) source = source.Reduce(Comparison);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to extract the appropriate tokens from the given source.
    /// <br/> This method must not invoke any reduce action.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected abstract IStrToken Extract(string source);
}
namespace Yotei.ORM.Internals.Code;

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
    }

    /// <inheritdoc/>
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;

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

        // It may happen that the things to search for are split between adjacent text elements,
        // so we need to combine them before extracting contents...

        if (source is StrTokenChain chain) source = chain.ReduceText();
        source = source.TokenizeWith(Extract);

        if (reduce) source = source.Reduce(Comparison);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to extract the appropriate tokens from the given source.
    /// <br/> This method MUST NOT invoke any reduce action.
    /// </summary>
    protected abstract IStrToken Extract(string source);
}
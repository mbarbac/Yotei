namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting string tokens from a given source.
/// </summary>
public abstract record StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public StrTokenizer() { }

    /// <summary>
    /// The comparison mode used to compare string sequences.
    /// </summary>
    public StringComparison Comparison { get; init; } = default!;

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given source using the rules defined by this instance. By default, the
    /// obtained result is then reduced before returning.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IStrToken Tokenize(string source, bool reduce = true)
    {
        var token = new StrTokenText(source);
        return Tokenize(token, reduce);
    }

    /// <summary>
    /// Tokenizes the given source using the rules defined by this instance. By default, the
    /// obtained result is then reduced before returning.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    /// It may happen that the given source is the result of a previous operation, which may
    /// have left splitted text elements. Hence why we combine them in case the source is a
    /// chain.
    public IStrToken Tokenize(IStrToken source, bool reduce = true)
    {
        source.ThrowWhenNull();

        if (source is StrTokenChain chain) source = chain.ReduceTextElements();

        source = source.TokenizeWith(Extract);
        if (reduce) source = source.Reduce(Comparison);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to extract appropriate tokens from the given source.
    /// <br/> This method MUST NOT reduce the returned token.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected abstract IStrToken Extract(string source);
}
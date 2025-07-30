namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents the ability of extracting string tokens from a given source.
/// </summary>
public record StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public StrTokenizer() { }

    /// <summary>
    /// The comparison mode to use to compare string sequences.
    /// </summary>
    public StringComparison Comparison { get; init; } = default;

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given source using the rules defined by this instance. By default, the
    /// preliminary result is reduced to its simplest form, if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IStrToken Tokenize(string source, bool reduce = true)
    {
        var token = new StrTokenText(source);
        return Tokenize(token, reduce);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given source using the rules defined by this instance. By default, the
    /// preliminary result is reduced to its simplest form, if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public IStrToken Tokenize(IStrToken source, bool reduce = true)
    {
        source.ThrowWhenNull();

        // It may happen the things to search for are split among adjacent elements, hence why
        // we may need to combine those text elements before proceeding...
        if (source is StrTokenChain chain) source = chain.ReduceText();

        source = source.TokenizeWith(Extract);
        if (reduce) source = source.Reduce(Comparison);
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to extract the appropriate tokens from the given string source.
    /// <br/> This method MUST NOT reduce the returned token.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual IStrToken Extract(string source) => new StrTokenText(source);
}
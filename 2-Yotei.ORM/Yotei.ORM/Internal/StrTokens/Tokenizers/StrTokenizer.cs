namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Extracts string tokens from a given source.
/// </summary>
public abstract partial class StrTokenizer
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public StrTokenizer() { }

    /// <summary>
    /// The comparison used to compare string sequences.
    /// <br/> Its default value is <see cref="StringComparison.CurrentCulture"/>.
    /// </summary>
    [With] public StringComparison Comparison { get; set; }

    /// <summary>
    /// Determines if the source element will be reduced <b><u>before</u></b> tokenization.
    /// <br/> The default value of this property is <c>true</c>.
    /// </summary>
    [With] public bool ReduceSource { get; set; } = true;

    /// <summary>
    /// Determines if the resulting token will be reduced <b><u>after</u></b> tokenization.
    /// <br/> The default value of this property is <c>true</c>.
    /// </summary>
    [With] public bool ReduceResult { get; set; } = true;

    // -----------------------------------------------------

    /// <summary>
    /// Tokenizes the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual IStrToken Tokenize(string source)
    {
        var token = new StrTokenText(source ?? string.Empty);
        return Tokenize(token);
    }

    /// <summary>
    /// Tokenizes the given source.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual IStrToken Tokenize(IStrToken token)
    {
        token.ThrowWhenNull();

        if (ReduceSource) token = token.Reduce(Comparison);
        token = token.Tokenize(Extract);
        if (ReduceResult) token = token.Reduce(Comparison);

        return token;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to extract the appropriate tokens from the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected abstract IStrToken Extract(string source);
}
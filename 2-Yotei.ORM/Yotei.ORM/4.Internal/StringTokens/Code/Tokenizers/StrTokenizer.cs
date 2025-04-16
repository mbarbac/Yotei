namespace Yotei.ORM.Internal;

// ========================================================
/// <inheritdoc cref="IStrTokenizer"/>
[With(InheritMembers = true)]
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
    protected StrTokenizer(StrTokenizer source) => Comparison = source.Comparison;

    /// <inheritdoc/>
    public StringComparison Comparison { get; init; } = StringComparison.CurrentCulture;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IStrToken Tokenize(string source, bool reduce = true)
    {
        throw null;
    }

    /// <inheritdoc/>
    public IStrToken Tokenize(IStrToken source, bool reduce = true)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to extract the appropriate tokens from the given source string.
    /// <br/> This method must not invoke any reduce action.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected abstract IStrToken Extract(string source);

    // ----------------------------------------------------

    /// <summary>
    /// Provides the ability of reducing the given token to a simpler form, if possible.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual IStrToken Reduce(IStrToken token)
    {
        throw null;
    }
}
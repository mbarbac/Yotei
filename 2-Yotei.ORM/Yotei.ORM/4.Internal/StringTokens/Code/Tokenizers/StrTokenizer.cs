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
        var token = new StrTokenText(source);
        return Tokenize(token, reduce);
    }

    /// <inheritdoc/>
    public IStrToken Tokenize(IStrToken token, bool reduce = true)
    {
        token.ThrowWhenNull();

        token = ReduceTextTokens(token);
        token = token.TokenizeWith(Extract);
        if (reduce) token = token.Reduce(Comparison);

        return token;
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
    public abstract IStrToken Reduce(IStrToken token);

    // ----------------------------------------------------

    /// <summary>
    /// Provides basic reduce capabilities by combining text element and returning a simpler
    /// form, if possible.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static IStrToken ReduceTextTokens(IStrToken token)
    {
        // Chain sources...
        if (token is StrTokenChain chain)
        {
            var builder = chain.GetBuilder();
            var changed = false;

            // Combining text elements starting from [1], not from [0]...
            for (int i = 1; i < builder.Count; i++)
            {
                var prev = builder[i - 1];
                var item = builder[i];

                if (prev is StrTokenText xprev && item is StrTokenText xitem)
                {
                    builder[i - 1] = new StrTokenText($"{xprev.Payload}{xitem.Payload}");
                    builder.RemoveAt(i);
                    i--;
                    changed = true;
                }
            }

            // Simplifying...
            token =
                builder.Count == 0 ? StrTokenText.Empty :
                builder.Count == 1 ? builder[0] :
                changed ? builder.ToInstance() : chain;
        }

        // Finishing...
        return token;
    }
}
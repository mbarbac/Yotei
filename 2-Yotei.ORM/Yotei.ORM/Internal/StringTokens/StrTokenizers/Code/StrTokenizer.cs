﻿namespace Yotei.ORM.Internal;

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

        // We reduce text tokens up-front as the things to search for might be a combination
        // of two consecutive text elements...
        token = ReduceTextTokens(token);
        token = token.TokenizeWith(Extract);

        // But we only reduce after finishing if such is requested...
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
    /// Provides basic reduce capabilities by combining text element and returning a simpler
    /// form, if possible.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static IStrToken ReduceTextTokens(IStrToken token)
    {
        // Chain sources...
        if (token is IStrTokenChain chain)
        {
            // Only combining if needed, to save GC allocations...
            if (chain.Count(x => x is IStrTokenText) > 1)
            {
                var builder = chain.GetBuilder();
                var changed = false;

                // Combining starting from [1], not from [0]...
                for (int i = 1; i < builder.Count; i++)
                {
                    var prev = builder[i - 1];
                    var item = builder[i];

                    if (prev is IStrTokenText xprev && item is IStrTokenText xitem)
                    {
                        builder[i - 1] = new StrTokenText($"{xprev.Payload}{xitem.Payload}");
                        builder.RemoveAt(i);
                        i--;
                        changed = true;
                    }
                }

                // Recreating if needed...
                if (changed) chain = builder.ToInstance();
            }

            // And, in any case, simplifying if possible...
            token =
                chain.Count == 0 ? StrTokenText.Empty :
                chain.Count == 1 ? chain[0] :
                chain;
        }

        // Finishing...
        return token;
    }
}
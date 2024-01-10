namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an indexed getter on a token.
/// </summary>
public sealed class TokenIndexed : TokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public TokenIndexed(Token host, IEnumerable<Token> indexes) : base(host)
    {
        Indexes = new TokenChain(indexes);

        if (Indexes.Count == 0) throw new ArgumentException(
            "Collection of indexes cannot be an empty one.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Host}{Indexes.ToString('[', ", ", ']')}";

    /// <summary>
    /// The indexes used by this instance.
    /// </summary>
    public TokenChain Indexes { get; }
}
namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries a single-part identifier.
/// </summary>
public sealed class TokenIdentifier : TokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="part"></param>
    public TokenIdentifier(Token host, IIdentifierPart identifier) : base(host)
    {
        IdentifierPart = identifier.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    public TokenIdentifier(Token host, IIdentifier identifier) : base(host)
    {
        identifier.ThrowWhenNull();

        if (identifier is IIdentifierPart part) IdentifierPart = part;
        else if (identifier.Count == 0) IdentifierPart = Code.IdentifierPart.Empty;
        else
        {
            for (int i = 0; i < identifier.Count - 1; i++)
            {
                var item = identifier[i];
                host = new TokenIdentifier(host, item);
            }
            IdentifierPart = identifier[^1];
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Host}.{IdentifierPart}";

    /// <summary>
    /// The single-part identifier carried by this instance.
    /// </summary>
    public IIdentifierPart IdentifierPart { get; }

    /// <summary>
    /// Determines if, up to this instance, the chain of tokens represents a pure identifier,
    /// or not.
    /// </summary>
    public bool IsPureIdentifier => Host switch
    {
        TokenArgument => true,
        TokenIdentifier => true,
        _ => false,
    };
}
namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries a single-part identifier.
/// </summary>
public sealed class TokenIdentifier : TokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> If the given identifier is not a single-part one, then an appropriate chain of
    /// host single-part identifiers is created.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    public TokenIdentifier(Token host, IIdentifier identifier) : base(host)
    {
        identifier.ThrowWhenNull();

        if (identifier is IIdentifierPart part) Identifier = part;
        else
        {
            var chain = (IIdentifierChain)identifier;
            var engine = chain.Engine;

            if (chain.Count == 0 || chain.Value == null) Identifier = new IdentifierPart(engine);
            else
            {
                for (int i = 0; i < chain.Count - 1; i++)
                {
                    var item = chain[i];
                    host = new TokenIdentifier(host, item);
                }
                Identifier = chain[^1];
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Host}.{Identifier}";

    /// <summary>
    /// The single-part identifier carried by this instance.
    /// </summary>
    public IIdentifierPart Identifier { get; }

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
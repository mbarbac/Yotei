namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents a hosted token.
/// </summary>
public abstract class TokenHosted : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public TokenHosted(Token host) => Host = host.ThrowWhenNull();

    /// <summary>
    /// The host of this instance.
    /// </summary>
    public Token Host { get; }

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() => Host.GetArgument();
}
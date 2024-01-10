namespace Yotei.ORM.Internal;

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
    [SuppressMessage("", "IDE0290")]
    public TokenHosted(Token host) => Host = host.ThrowWhenNull();

    /// <summary>
    /// The host of this instance.
    /// </summary>
    public Token Host { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => Host.GetArgument();
}
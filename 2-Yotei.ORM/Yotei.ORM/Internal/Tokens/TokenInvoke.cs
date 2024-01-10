namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the direct invocation of a given host.
/// <br/> When visited, instances of this class are typically used to inject its arguments into
/// the results being produced.
/// </summary>
public sealed class TokenInvoke : TokenHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public TokenInvoke(Token host) : this(host, []) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="arguments"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenInvoke(Token host, IEnumerable<Token> arguments) : base(host)
    {
        Arguments = new TokenChain(arguments);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var args = Arguments.Count == 0 ? "()" : Arguments.ToString('(', ", ", ')');
        return $"{Host}{args}";
    }

    /// <summary>
    /// The arguments used by this instance.
    /// </summary>
    public TokenChain Arguments { get; }
}
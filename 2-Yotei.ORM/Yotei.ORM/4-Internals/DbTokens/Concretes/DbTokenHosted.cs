namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary hosted token.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract class DbTokenHosted : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public DbTokenHosted(IDbToken host) => Host = host;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => Host.GetArgument();

    /// <summary>
    /// The host of this token.
    /// </summary>
    public IDbToken Host { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public abstract bool Equals(IDbToken? other);
}
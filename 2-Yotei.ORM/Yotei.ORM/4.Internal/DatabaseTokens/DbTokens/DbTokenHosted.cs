namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a hosted token.
/// </summary>
public abstract class DbTokenHosted : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public DbTokenHosted(DbToken host) => Host = host.ThrowWhenNull();

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => Host.GetArgument();

    /// <summary>
    /// The host of this instance.
    /// </summary>
    public DbToken Host { get; }
}
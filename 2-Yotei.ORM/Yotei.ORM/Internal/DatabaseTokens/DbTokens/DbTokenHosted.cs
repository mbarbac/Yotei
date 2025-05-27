namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a hosted token.
/// </summary>
[Cloneable]
public abstract partial class DbTokenHosted : DbToken
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
    public DbToken Host { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// Changes the host of this instance with the given one.
    /// <br/>
    /// This method modifies this instance breaking its immutability, so caution is advised and,
    /// in most circumstances, you should take a clone before using this method.
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public virtual void ChangeHost(DbToken host) => Host = host.ThrowWhenNull();
}
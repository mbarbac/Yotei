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
    /// Returns a clone of this instance where the original host has been replaced with the given
    /// one.
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public virtual DbTokenHosted ChangeHost(DbToken host)
    {
        host.ThrowWhenNull();

        if (ReferenceEquals(Host, host)) return this;

        var temp = Clone(); temp.Host = host;
        return temp;
    }
}
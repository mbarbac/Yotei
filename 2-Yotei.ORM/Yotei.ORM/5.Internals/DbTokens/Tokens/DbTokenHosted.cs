namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary hosted token.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract class DbTokenHosted : IDbToken
{
    // <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public DbTokenHosted(IDbToken host) => Host = host;

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => Host.GetArgument();

    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract DbTokenHosted Clone();
    IDbToken IDbToken.Clone() => Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract bool Equals(IDbToken? other);

    // ----------------------------------------------------

    /// <summary>
    /// The host of this instance.
    /// </summary>
    /// The setter is provided to enable advance internal scenarios, but actually it modifies
    /// the semantics of this instance, breaking immutability. Use at your own risk.
    public IDbToken Host
    {
        get => _Host;
        internal set => _Host = value.ThrowWhenNull();
    }
    IDbToken _Host = default!;
}
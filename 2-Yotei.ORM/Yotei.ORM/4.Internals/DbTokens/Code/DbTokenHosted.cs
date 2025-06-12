namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a hosted token.
/// </summary>
[Cloneable]
public abstract partial class DbTokenHosted : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public DbTokenHosted(IDbToken host) => Host = host;

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => Host.GetArgument();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract bool Equals(IDbToken? other);

    // ----------------------------------------------------

    /// <summary>
    /// The host of this instance.
    /// </summary>
    public IDbToken Host
    {
        get => _Host;
        init => _Host = value.ThrowWhenNull();
    }
    IDbToken _Host = default!;
}
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

        /// <remarks>
        /// The setter is provided to allow advance scenarios, but it only does basic (not null)
        /// validations of the given value. Also, it DOES NOT create a copy but rather modifies
        /// this instance, so use with CAUTION.
        /// </remarks>
        internal set => _Host = value.ThrowWhenNull();
    }
    IDbToken _Host = default!;
}
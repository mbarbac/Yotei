namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a dynamic argument in a dynamic lambda expression. Instances of this type are
/// considered translation artifacts, with no representation in a database command.
/// </summary>
public class DbTokenArgument : IDbToken
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    public virtual DbTokenArgument Clone() => throw null;
    IDbToken IDbToken.Clone() => Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }
}
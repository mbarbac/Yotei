namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a dynamic argument in a dynamic lambda expression. 
/// <b/> Instances of this type are considered translation artifacts, with no representation
/// in a database command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenArgument : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenArgument(string name) => Name = DbToken.ValidateTokenName(name);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenArgument(DbTokenArgument source) : this(source.Name) { }

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public virtual DbTokenArgument Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenArgument valid) return false;

        if (Name == valid.Name) return true;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenArgument? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenArgument? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }
}
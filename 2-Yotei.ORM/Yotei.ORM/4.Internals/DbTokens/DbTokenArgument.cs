namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a dynamic argument in a dynamic lambda expression.  Instances of this type are
/// considered translation artifacts, with no representation in a database command.
/// </summary>
[Cloneable]
public partial class DbTokenArgument : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenArgument(string name) => Name = ValidateTokenName(name);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenArgument(DbTokenArgument source) : this(source.Name) { }

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => this;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenArgument valid) return false;

        if (Name == valid.Name) return true;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenArgument? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenArgument? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Name.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }
}
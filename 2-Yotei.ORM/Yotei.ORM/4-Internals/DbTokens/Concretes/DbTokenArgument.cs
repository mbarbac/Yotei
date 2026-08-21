namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a dynamic argument in a dynamic lambda expression. Instances of this type are
/// considered translation artifacts, with no text representation in a database command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial class DbTokenArgument : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DbTokenArgument(string name) => Name = DbToken.ValidateTokenName(name);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected DbTokenArgument(DbTokenArgument other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Name = other.Name;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument() => this;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenArgument valid) return false;

        if (Name == valid.Name) return true;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenArgument);

    public static bool operator ==(DbTokenArgument? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenArgument? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Name);
        return code;
    }
}
namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary literal in a database expression that, by convention, will never be
/// captured as an argument.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial class DbTokenLiteral : IDbToken
{
    /// <summary>
    /// Represents an empty literal.
    /// </summary>
    public static DbTokenLiteral Empty { get; } = new(string.Empty);

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DbTokenLiteral(string value) => Value = value.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenLiteral(DbTokenLiteral source) : this(source.Value) { }

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <inheritdoc cref="ICloneable.Clone"/>
    public virtual DbTokenLiteral Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenLiteral valid) return false;

        if (Value != valid.Value) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenLiteral? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenLiteral? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The actual not-null literal value carried by this instance.
    /// <br/> Empty values are considered valid ones.
    /// </summary>
    public string Value { get; }
}
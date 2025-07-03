namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an indexed getter on a given host token.
/// </summary>
[Cloneable]
public partial class DbTokenIndexed : DbTokenHosted
{
    /// <summary>
    /// Initialises a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public DbTokenIndexed(IDbToken host, IEnumerable<IDbToken> indexes) : base(host)
    {
        indexes.ThrowWhenNull();
        Indexes = new(indexes);

        if (Indexes.Count == 0) throw new ArgumentException(
            "Collection of indexes cannot be an empty one.");
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenIndexed(DbTokenIndexed source) : this(
        source.Host.Clone(),
        source.Indexes.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString() => $"{Host}{Indexes.ToString(rounded: false)}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenIndexed valid) return false;

        if (!Indexes.Equals(valid.Indexes)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenIndexed? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenIndexed? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Indexes.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The indexes used by this instance, which are not an empty collection.
    /// </summary>
    public DbTokenChain Indexes { get; }
}
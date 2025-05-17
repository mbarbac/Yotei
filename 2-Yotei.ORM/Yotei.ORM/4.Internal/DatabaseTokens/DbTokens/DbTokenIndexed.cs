namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an indexed getter on a given host token.
/// </summary>
public class DbTokenIndexed : DbTokenHosted
{
    /// <summary>
    /// Initialises a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public DbTokenIndexed(DbToken host, IEnumerable<DbToken> indexes) : base(host)
    {
        Indexes = new(indexes);

        if (Indexes.Count == 0) throw new ArgumentException(
            "Collection of indexes cannot be an empty one.");
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Host}{Indexes.ToString(false)}";

    /// <summary>
    /// The indexes used by this instance.
    /// </summary>
    public DbTokenChain Indexes { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenIndexed xother)
        {
            if (Indexes.Equals(xother.Indexes)) return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Indexes.GetHashCode();
}
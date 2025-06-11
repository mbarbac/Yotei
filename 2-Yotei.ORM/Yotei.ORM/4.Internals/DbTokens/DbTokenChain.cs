namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an ordered and flat collection of arbitrary tokens.
/// </summary>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<DbToken>]
public partial class DbTokenChain : DbToken
{
    /// <inheritdoc/>
    protected override Builder Items { get; }

    /// <summary>
    /// Invoked to create the initial repository of contents of this instance.
    /// </summary>
    protected virtual Builder OnInitialize() => new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public DbTokenChain() => Items = OnInitialize();

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public DbTokenChain(IEnumerable<DbToken> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenChain(DbTokenChain source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(DbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenChain valid) return false;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i]);
            if (!equal) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(DbTokenChain? host, DbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(DbTokenChain? host, DbToken? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public virtual Builder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => throw null;
}
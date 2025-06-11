using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a flat and ordered collection of <see cref="IDbToken"/> instances.
/// </summary>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IDbToken>]
public partial class DbTokenChain : IDbToken
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
    public DbTokenChain(IEnumerable<IDbToken> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenChain(DbTokenChain source) => Items = source.Items.Clone();

    /// <inheritdoc cref="ICloneable.Clone"/>
    public override DbTokenChain Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns the collection-alike string representation of this instance, wrapped with
    /// squared or rounded brackets as requested.
    /// </summary>
    /// <param name="rounded"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public string ToString(bool rounded, string separator = ", ") => Items.ToString(rounded, separator);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
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
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenChain? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(DbTokenChain? host, IDbToken? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder based upon the contents in this instance.
    /// </summary>
    /// <returns></returns>
    public virtual Builder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the reduction of this instance to a simpler form, if possible.
    /// </summary>
    /// <returns></returns>
    public IDbToken Reduce() =>
        Count == 0 ? DbTokenLiteral.Empty :
        Count == 1 ? this[0] :
        this;
}
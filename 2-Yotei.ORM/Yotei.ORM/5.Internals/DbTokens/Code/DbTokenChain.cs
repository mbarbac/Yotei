namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a flat ordered collection of <see cref="IDbToken"/> instances.
/// </summary>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IDbToken>]
public partial class DbTokenChain : IDbToken
{
    protected override Builder Items { get; }
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
    public override DbTokenChain Clone()
    {
        var chain = new DbTokenChain();

        foreach (var item in this) chain.Items.Add(item.Clone());
        return chain;
    }
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns a string representation of this instance using the rounded or square brackets,
    /// as requested, and with the given separator among elements.
    /// </summary>
    /// <param name="rounded"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public string ToString(
        bool rounded, string separator = ", ") => Items.ToString(rounded, separator);

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

    public static bool operator ==(DbTokenChain? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenChain? host, IDbToken? other) => !(host == other);

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
    public DbTokenArgument? GetArgument()
    {
        for (int i = 0; i < Count; i++)
        {
            var arg = Items[i].GetArgument();
            if (arg is not null) return arg;
        }
        return null;
    }

    /// <summary>
    /// Reduces this instance to a simpler form, if possible.
    /// </summary>
    /// <returns></returns>
    public IDbToken Reduce()
    {
        return
            Count == 0 ? DbTokenLiteral.Empty :
            Count == 1 ? this[0] :
            this;
    }
}
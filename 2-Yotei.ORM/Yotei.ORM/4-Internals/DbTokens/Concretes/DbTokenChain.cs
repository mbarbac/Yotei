namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a flat and ordered collection of arbitrary tokens.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString(3)}")]
[InvariantList<IDbToken>]
public partial class DbTokenChain : IDbToken
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public DbTokenChain() => Items = [];

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    [SuppressMessage("", "IDE0028")]
    public DbTokenChain(IEnumerable<IDbToken> range) => Items = new(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("", "IDE0028")]
    public override DbTokenChain Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    [SuppressMessage("", "IDE0028")]
    protected DbTokenChain(DbTokenChain other) => Items = new(other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns an alternate string representation of this instance.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public string ToString(
        string head, string tail, string separator = ", ") => Items.ToString(head, tail, separator);

    /// <summary>
    /// <inheritdoc/>
    /// <br/> This method returns the first argument found from its collection of elements, or
    /// null if it is an empty one, or any can be found.
    /// </summary>
    /// <returns></returns>
    public DbTokenArgument? GetArgument()
    {
        for (int i = 0; i < Count; i++)
        {
            var arg = Items[i].GetArgument();
            if (arg is not null) return arg;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override Builder ToBuilder() => Items.Clone();

    /// <summary>
    /// Reduces this instance to a simpler form, if possible, or otherwise returns this instance.
    /// </summary>
    /// <returns></returns>
    public IDbToken Reduce() =>
        Count == 0 ? DbTokenLiteral.Empty :
        Count == 1 ? Items[0] :
        this;

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
        if (other is not DbTokenChain valid) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid.Items[i];
            var same = item.Equals(temp);
            if (!same) return false;
        }
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as DbTokenChain);

    public static bool operator ==(DbTokenChain? host, IDbToken? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;
        return host.Equals(item);
    }

    public static bool operator !=(DbTokenChain? host, IDbToken? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
}
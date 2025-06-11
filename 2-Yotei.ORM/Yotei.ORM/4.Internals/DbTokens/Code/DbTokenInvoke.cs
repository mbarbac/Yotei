namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a direct invocation on a given host token.
/// </summary>
[Cloneable]
public partial class DbTokenInvoke : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance with empty arguments.
    /// </summary>
    /// <param name="host"></param>
    public DbTokenInvoke(IDbToken host) : base(host) => Arguments = new();

    /// <summary>
    /// Initializes a new instance with the given arguments.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="args"></param>
    public DbTokenInvoke(IDbToken host, IEnumerable<IDbToken> args) : base(host)
    {
        Arguments = new(args);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenInvoke(DbTokenInvoke source) : this(
        source.Host.Clone(),
        source.Arguments.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var args = Arguments.ToString(true);
        return $"{Host}{args}";
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenInvoke valid) return false;

        if (!Arguments.Equals(valid.Arguments)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenInvoke? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenInvoke? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Arguments.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The arguments used by this instance, which may be an empty collection.
    /// </summary>
    public DbTokenChain Arguments { get; }
}
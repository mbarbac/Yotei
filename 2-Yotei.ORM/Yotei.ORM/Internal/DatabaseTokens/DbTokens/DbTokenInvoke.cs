namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a direct invocation on a given host token.
[Cloneable]
public partial class DbTokenInvoke : DbTokenHosted
{
    /// <summary>
    /// Initializes a new parameter-less instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public DbTokenInvoke(DbToken host, string name) : this(host, []) { }

    /// <summary>
    /// Initializes a new instance with the regular arguments.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="args"></param>
    public DbTokenInvoke(DbToken host, IEnumerable<DbToken> args) : base(host)
    {
        Arguments = new(args);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public DbTokenInvoke(DbTokenInvoke source) : this(source.Host, source.Arguments) { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var args = Arguments.ToString(true);
        return $"{Host}{args}";
    }

    /// <summary>
    /// The arguments used by this instance, which may be an empty collection.
    /// </summary>
    public DbTokenChain Arguments { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenInvoke xother)
        {
            if (Arguments.Equals(xother.Arguments)) return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Arguments.GetHashCode();
}
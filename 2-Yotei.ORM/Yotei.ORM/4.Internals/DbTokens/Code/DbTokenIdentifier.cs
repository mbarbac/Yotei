namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries a single-part identifier.
/// </summary>
[Cloneable]
public partial class DbTokenIdentifier : DbTokenHosted
{
    /// <summary>
    /// Returns a new instance where if the given identifier is a multi-part one, an appropriate
    /// chain of hosts is built using its parts. Otherwise, reverts to a standard creation.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public static DbTokenIdentifier Create(IDbToken host, IIdentifier identifier)
    {
        host.ThrowWhenNull();
        identifier.ThrowWhenNull();

        if (identifier is IIdentifierPart part) return new(host, part);

        var engine = identifier.Engine;
        var chain = (IIdentifierChain)identifier;

        if (chain.Count == 0 || chain.Value is null) return new(host, new IdentifierPart(engine));

        for (int i = 0; i < chain.Count; i++)
        {
            part = chain[i];
            host = new DbTokenIdentifier(host, part);
        }

        return (DbTokenIdentifier)host;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance with the given host and single-part identifier.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    public DbTokenIdentifier(IDbToken host, IIdentifierPart identifier)
        : base(host)
        => Identifier = identifier.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenIdentifier(DbTokenIdentifier source) : this(
        source.Host.Clone(),
        source.Identifier)
    { }

    /// <inheritdoc/>
    public override string ToString() => $"{Host}.{Identifier}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenIdentifier valid) return false;

        var engine = Identifier.Engine;
        var sensitive = engine.CaseSensitiveNames;
        if (!Identifier.Equals(valid.Identifier, sensitive)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenIdentifier? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenIdentifier? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Identifier.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The single-part identifier carried by this instance.
    /// </summary>
    public IIdentifierPart Identifier { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    public string? Value => Identifier.Value;

    /// <summary>
    /// Determines if this instance, along with its chain of hosts, represents a pure identifier
    /// or not.
    /// </summary>
    public bool IsPureIdentifier => Host switch
    {
        DbTokenArgument => true,
        DbTokenIdentifier parent => parent.IsPureIdentifier,
        _ => false,
    };
}
namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries a single-part identifier.
/// </summary>
[Cloneable]
public partial class DbTokenIdentifier : DbTokenHosted
{
    /// <summary>
    /// Initializes a new instance based upon the given identifier.
    /// <br/> If it is not a single-part one, then a suitable chain of single-part identifiers is
    /// built from the given host.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    //public DbTokenIdentifier(IDbToken host, IIdentifier identifier) : base(host)
    //{
    //    identifier.ThrowWhenNull();

    //    if (identifier is IIdentifierPart part) Identifier = part;
    //    else
    //    {
    //        var engine = identifier.Engine;
    //        var chain = (IIdentifierChain)identifier;

    //        if (chain.Count == 0 || chain.Value == null) Identifier = new IdentifierPart(engine);
    //        else
    //        {
    //            for (int i = 0; i < (chain.Count - 1); i++)
    //            {
    //                var item = chain[i];

    //            }
    //            Identifier = chain[^1];
    //        }
    //    }
    //}

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
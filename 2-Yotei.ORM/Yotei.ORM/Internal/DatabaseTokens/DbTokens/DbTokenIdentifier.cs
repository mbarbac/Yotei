namespace Yotei.ORM.Internal;

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
    public DbTokenIdentifier(DbToken host, IIdentifier identifier) : base(host)
    {
        identifier.ThrowWhenNull();

        if (identifier is IIdentifierPart part) Identifier = part;
        else
        {
            var engine = identifier.Engine;
            var chain = (IIdentifierChain)identifier;

            if (chain.Count == 0 || chain.Value == null) Identifier = new IdentifierPart(engine);
            else
            {
                for (int i = 0; i < (chain.Count - 1); i++)
                {
                    var item = chain[i];
                    host = new DbTokenIdentifier(host, item);
                }
                Identifier = chain[^1];
            }
        }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public DbTokenIdentifier(DbTokenIdentifier source) : this(source.Host, source.Identifier) { }

    /// <inheritdoc/>
    public override string ToString() => $"{Host}.{Identifier}";

    /// <inheritdoc/>
    public override DbTokenIdentifier ChangeHost(
        DbToken host) => (DbTokenIdentifier)base.ChangeHost(host);

    /// <summary>
    /// The single-part identifier carried by this instance.
    /// </summary>
    public IIdentifierPart Identifier { get; }

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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenIdentifier xother)
        {
            return Identifier.Equals(xother.Identifier);
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Identifier.GetHashCode();
}
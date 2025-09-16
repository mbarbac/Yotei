namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries a single-part identifier.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenIdentifier : DbTokenHosted
{
    /// <summary>
    /// Returns a new instance using the given host and identifier. If the identifier is not a
    /// single-part one, a chain of host instances is built using the given chain of identifiers.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="identifier"></param>
    public DbTokenIdentifier(IDbToken host, IIdentifier identifier) : base(host)
    {
        identifier.ThrowWhenNull();

        if (identifier is IIdentifierUnit part) Identifier = part;
        else
        {
            var engine = identifier.Engine;
            var chain = (IIdentifierChain)identifier;

            if (chain.Count == 0 || chain.Value is null) Identifier = new IdentifierUnit(engine);
            else
            {
                for (int i = 0; i < chain.Count; i++)
                {
                    Identifier = chain[i];
                    Host = host;

                    host = new DbTokenIdentifier(host, Identifier);
                }
            }
        }
    }

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

    /// <inheritdoc/>
    public override DbTokenIdentifier Clone() => new(this);

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
    public IIdentifierUnit Identifier { get; } = default!;

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// </summary>
    public string? Value => Identifier.Value;

    /// <summary>
    /// Determines if this instance, along with its chain of hosts, represents a pure identifier.
    /// or not.
    /// </summary>
    public bool IsPureIdentifier => Host switch
    {
        DbTokenArgument => true,
        DbTokenIdentifier parent => parent.IsPureIdentifier,
        _ => false,
    };
}
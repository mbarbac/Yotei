namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries an arbitrary command-info instance.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class DbTokenCommandInfo : IDbToken
{
    /// <summary>
    /// Initializes a new instance with a clone of the given command.
    /// </summary>
    /// <param name="info"></param>
    public DbTokenCommandInfo(ICommandInfo info) => CommandInfo = info.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCommandInfo(DbTokenCommandInfo source) : this(source.CommandInfo) { }

    /// <inheritdoc/>
    public override string ToString() => CommandInfo.IsEmpty ? string.Empty : CommandInfo.Text;

    /// <inheritdoc cref="ICloneable.Clone"/>
    public virtual DbTokenCommandInfo Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommandInfo valid) return false;

        return CommandInfo.Equals(valid.CommandInfo);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenCommandInfo? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCommandInfo? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => CommandInfo.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The command info carried by this instance.
    /// </summary>
    public ICommandInfo CommandInfo { get; }
}
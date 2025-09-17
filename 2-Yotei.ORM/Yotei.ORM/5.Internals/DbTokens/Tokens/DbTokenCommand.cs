namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that carries a reference to an arbitrary command instance.
/// <br/> Note that despite that reference is immutable the command itself may be not, which
/// somehow breaks the immutability semantics. For that reason, by default, the carried command
/// is a clone of the original one.
/// </summary>
public class DbTokenCommand : IDbToken
{
    /// <summary>
    /// Initializes a new instance for a command, which by default is a clone of the original
    /// given one.
    /// </summary>
    /// <param name="command"></param>
    public DbTokenCommand(ICommand command, bool clone = true) => Command = clone
        ? command.ThrowWhenNull().Clone()
        : command.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCommand(DbTokenCommand source) : this(source.Command) { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var info = Command.GetCommandInfo(iterable: false);
        return info.IsEmpty ? string.Empty : $"({info.Text})";
    }

    /// <inheritdoc cref="ICloneable.Clone"/>
    public virtual DbTokenCommand Clone() => new(this);
    IDbToken IDbToken.Clone() => Clone();

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommand valid) return false;

        return Command.Equals(valid.Command);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenCommand? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCommand? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Command.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The command carried by this instance.
    /// </summary>
    public ICommand Command { get; }
}
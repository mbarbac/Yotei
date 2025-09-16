namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents a token that represents an arbitrary command instance.
/// <para>
/// Note that instances of this type carries a reference to a given <see cref="ICommand"/> and,
/// even if the reference itself is immutable, the underlying command may be not. This breaks
/// somehow the immutability semantics and so instances of this type only should be used in
/// controlled scenarios.
/// </para>
/// </summary>
public class DbTokenCommand : IDbToken
{
    /// <summary>
    /// Initializes a new instance with a clone of the given command.
    /// </summary>
    /// <param name="command"></param>
    public DbTokenCommand(ICommand command) => Command = command.ThrowWhenNull();

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
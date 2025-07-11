namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an embedded command in a database expression.
/// </summary>
[Cloneable]
public partial class DbTokenCommand : IDbToken
{
    /// <summary>
    /// Initializes a new instance with a clone of the given command.
    /// </summary>
    /// <param name="command"></param>
    public DbTokenCommand(ICommand command) => Command = command.ThrowWhenNull().Clone();

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

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommand valid) return false;

        var tinfo = Command.GetCommandInfo(iterable: false);
        var vinfo = valid.Command.GetCommandInfo(iterable: false);

        return tinfo.Equals(vinfo);
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
    /// <br/> Its value is a clone of the original command given to the constructor, captured at
    /// that time. It is not intended to be modified.
    /// </summary>
    public ICommand Command { get; }
}
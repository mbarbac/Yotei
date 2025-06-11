namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an embedded command in a database expression.
/// </summary>
[Cloneable]
public partial class DbTokenCommand : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public DbTokenCommand(ICommand command) => Command = command.ThrowWhenNull().Clone();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCommand(DbTokenCommand source) : this(
        source.Command.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var info = Command.GetCommandInfo(iterable: false);
        var str = info.Text.UnWrap('(', ')', trim: true, recursive: true);
        return $"({str})";
    }

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommand valid) return false;

        var tinfo = Command.GetCommandInfo(iterable: false);
        var xinfo = valid.Command.GetCommandInfo(iterable: false);

        if (tinfo.IsEmpty || xinfo.IsEmpty) return false;
        if (!tinfo.Engine.Equals(xinfo.Engine)) return false;
        if (string.Compare(tinfo.Text, xinfo.Text, !tinfo.Engine.CaseSensitiveNames) != 0) return false;
        if (!tinfo.Parameters.Equals(xinfo.Parameters)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenCommand? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCommand? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Command.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The embedded command carried by this instance.
    /// <br/> This command is a clone of the original one as IT IS NOT INTENDED for modifications.
    /// </summary>
    public ICommand Command { get; }
}
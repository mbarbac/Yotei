namespace Yotei.ORM.Internals;

// ========================================================
/// Represents an embedded command in a database expression.
/// </summary>
[Cloneable]
public partial class DbTokenCommand : DbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public DbTokenCommand(ICommand command)
        => CommandInfo = command.ThrowWhenNull().GetCommandInfo(iterable: false);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="info"></param>
    public DbTokenCommand(ICommandInfo info) => CommandInfo = info.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCommand(DbTokenCommand source) : this(
        source.CommandInfo.Clone())
    { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = CommandInfo.Text.UnWrap('(', ')', trim: true, recursive: true);
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

        if (CommandInfo.IsEmpty != valid.CommandInfo.IsEmpty) return false;
        if (!CommandInfo.Engine.Equals(valid.CommandInfo.Engine)) return false;

        var sensitive = CommandInfo.Engine.CaseSensitiveNames;
        if (string.Compare(CommandInfo.Text, valid.CommandInfo.Text, !sensitive) != 0) return false;
        if (!CommandInfo.Parameters.Equals(valid.CommandInfo.Parameters)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as DbToken);

    public static bool operator ==(
        DbTokenCommand? host, DbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCommand? host, DbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => CommandInfo.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The info about the embedded command carried by this instance.
    /// </summary>
    public ICommandInfo CommandInfo { get; }
}
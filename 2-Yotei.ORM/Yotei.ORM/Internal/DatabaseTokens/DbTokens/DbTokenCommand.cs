namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an embedded command in a database expression.
/// <br/> The value carried by this instance is the immutable <see cref="CommandInfo"/> object
/// that carries the command's information.
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
    public DbTokenCommand(DbTokenCommand source) : this(source.CommandInfo) { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = CommandInfo.Text.UnWrap('(', ')', trim: true, recursive: true);
        return $"({str})";
    }

    /// <summary>
    /// The info about embedded command carried by this instance.
    /// </summary>
    public ICommandInfo CommandInfo { get; }

    /// <inheritdoc/>
    public override DbTokenArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Equals(DbToken? other)
    {
        if (other is DbTokenCommand xother)
        {
            if (CommandInfo.IsEmpty && xother.CommandInfo.IsEmpty) return true;

            if (CommandInfo.Text == xother.CommandInfo.Text &&
                CommandInfo.Parameters.Equals(xother.CommandInfo.Parameters))
                return true;
        }
        return ReferenceEquals(this, other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => CommandInfo.GetHashCode();
}
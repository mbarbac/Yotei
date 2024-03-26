namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents an embedded command in a database expression.
/// </summary>
public sealed class TokenCommand : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public TokenCommand(ICommandInfo command) => Command = command.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = Command.Text.UnWrap('(', ')', trim: true, recursive: true);
        return $"({str})";
    }

    /// <summary>
    /// The embedded command carried by this instance.
    /// </summary>
    public ICommandInfo Command { get; }

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() => null;
}
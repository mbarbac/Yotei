namespace Yotei.ORM.Internal;

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
    [SuppressMessage("", "IDE0290")]
    public TokenCommand(ICommand command) => Command = command.ThrowWhenNull().Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var str = Command.GetText(out _).UnWrap('(', ')', trim: true, recursive: true);
        return $"({str})";
    }

    /// <summary>
    /// The embedded command carried by this instance. Its actual value is a clone of the one
    /// passed as an argument of the constructor.
    /// </summary>
    public ICommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => null;
}
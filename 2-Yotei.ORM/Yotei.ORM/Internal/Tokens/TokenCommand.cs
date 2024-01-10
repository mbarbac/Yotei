namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an embedded command inside a database expression.
/// </summary>
public sealed class TokenCommand : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenCommand(ICommand command) => Command = command.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Command.GetText(out _);

    /// <summary>
    /// The actual embedded command carried by this instance.
    /// </summary>
    public ICommand Command { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() => null;
}
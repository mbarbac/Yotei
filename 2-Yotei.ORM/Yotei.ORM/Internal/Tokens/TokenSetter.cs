namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the assignation of an arbitrary token to a target one.
/// </summary>
public sealed class TokenSetter : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    [SuppressMessage("", "IDE0290")]
    public TokenSetter(Token target, Token value)
    {
        Target = target.ThrowWhenNull();
        Value = value.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({Target} = {Value})";

    /// <summary>
    /// The target operand of this assignation operation.
    /// </summary>
    public Token Target { get; }

    /// <summary>
    /// The value operand of this assignation operation.
    /// </summary>
    public Token Value { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override TokenArgument? GetArgument() =>
        Target.GetArgument() ??
        Value.GetArgument();
}
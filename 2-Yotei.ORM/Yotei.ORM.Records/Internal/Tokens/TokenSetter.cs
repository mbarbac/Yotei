namespace Yotei.ORM.Records.Internal;

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
    public TokenSetter(Token target, Token value)
    {
        Target = target.ThrowWhenNull();
        Value = value.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"({Target} = {Value})";

    /// <summary>
    /// The target operand of this assignation operation.
    /// </summary>
    public Token Target { get; }

    /// <summary>
    /// The value operand of this assignation operation.
    /// </summary>
    public Token Value { get; }

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() =>
        Target.GetArgument() ??
        Value.GetArgument();
}
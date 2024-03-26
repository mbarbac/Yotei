namespace Yotei.ORM.Records.Internal;

// ========================================================
/// <summary>
/// Represents a conditional ternary operation (left ? middle : right).
/// </summary>
public sealed class TokenTernary : Token
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public TokenTernary(Token left, Token middle, Token right) : base()
    {
        Left = left.ThrowWhenNull();
        Middle = middle.ThrowWhenNull();
        Right = right.ThrowWhenNull();
    }

    /// <inheritdoc/>
    public override string ToString() => $"({Left} ? {Middle} : {Right})";

    /// <inheritdoc/>
    public override TokenArgument? GetArgument() =>
        Left.GetArgument() ??
        Middle.GetArgument() ??
        Right.GetArgument();

    /// <summary>
    /// The left operand of the ternary operation.
    /// </summary>
    public Token Left { get; }

    /// <summary>
    /// The middle operand of the ternary operation.
    /// </summary>
    public Token Middle { get; }

    /// <summary>
    /// The right operand of the ternary operation.
    /// </summary>
    public Token Right { get; }
}
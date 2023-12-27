namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic binary operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeBinary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public LambdaNodeBinary(LambdaNode left, ExpressionType operation, LambdaNode right) : base()
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaOperation = operation;
        LambdaRight = right.ThrowWhenNull();
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({LambdaLeft} {LambdaOperation} {LambdaRight})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeBinary Clone() => new(
        LambdaLeft.Clone(),
        LambdaOperation,
        LambdaRight.Clone());

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if any.
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() =>
        LambdaLeft.GetArgument() ??
        LambdaRight.GetArgument();

    /// <summary>
    /// The left operand of the binary operation.
    /// </summary>
    public LambdaNode LambdaLeft { get; }

    /// <summary>
    /// The binary operation.
    /// </summary>
    public ExpressionType LambdaOperation { get; }

    /// <summary>
    /// The right operand of the binary operation.
    /// </summary>
    public LambdaNode LambdaRight { get; }
}
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
    public LambdaNodeBinary(LambdaNode left, ExpressionType operation, LambdaNode right)
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaOperation = operation;
        LambdaRight = right.ThrowWhenNull();
        PrintInitialized();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
    /// <inheritdoc/>
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
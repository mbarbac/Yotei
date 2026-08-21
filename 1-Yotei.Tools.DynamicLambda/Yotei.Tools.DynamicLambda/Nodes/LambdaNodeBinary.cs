namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic binary operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
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
        DLambdaLeft = left.ThrowWhenNull();
        DLambdaOperation = operation;
        DLambdaRight = right.ThrowWhenNull();
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({DLambdaLeft} {DLambdaOperation} {DLambdaRight})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument()
        => DLambdaLeft.GetArgument()
        ?? DLambdaRight.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaLeft { get; }

    /// <summary>
    /// The dynamic binary operation represented by this instance.
    /// <br/> The caller is responsable for setting an appropriate value.
    /// </summary>
    public ExpressionType DLambdaOperation { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaRight { get; }
}
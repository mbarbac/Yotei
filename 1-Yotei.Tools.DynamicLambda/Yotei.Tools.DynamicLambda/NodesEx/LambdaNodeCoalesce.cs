namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic coalesce operation (x.Alpha ?? x.Beta) in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeCoalesce : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public LambdaNodeCoalesce(LambdaNode left, LambdaNode right) : base()
    {
        DLambdaLeft = left.ThrowWhenNull();
        DLambdaRight = right.ThrowWhenNull();
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({DLambdaLeft} ?? {DLambdaRight})";

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
    /// The right operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaRight { get; }
}
namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic coalesce operation (x.Alpha ?? x.Beta) in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeCoalesce : DLambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public DLambdaNodeCoalesce(DLambdaNode left, DLambdaNode right) : base()
    {
        DLambdaLeft = left.ThrowWhenNull();
        DLambdaRight = right.ThrowWhenNull();
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
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
    public override DLambdaNodeArgument? GetArgument()
        => DLambdaLeft.GetArgument()
        ?? DLambdaRight.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaLeft { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaRight { get; }
}
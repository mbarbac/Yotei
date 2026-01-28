namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic ternary operation (x.Alpha ? x.Beta : x.Delta) in a chain of dynamic
/// operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeTernary : DLambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public DLambdaNodeTernary(DLambdaNode left, DLambdaNode middle, DLambdaNode right) : base()
    {
        DLambdaLeft = left.ThrowWhenNull();
        DLambdaMiddle = middle.ThrowWhenNull();
        DLambdaRight = right.ThrowWhenNull();
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({DLambdaLeft} ? {DLambdaMiddle} : {DLambdaRight})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override DLambdaNodeArgument? GetArgument()
        => DLambdaLeft.GetArgument()
        ?? DLambdaMiddle.GetArgument()
        ?? DLambdaRight.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaLeft { get; }

    /// <summary>
    /// The middle operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaMiddle { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaRight { get; }
}
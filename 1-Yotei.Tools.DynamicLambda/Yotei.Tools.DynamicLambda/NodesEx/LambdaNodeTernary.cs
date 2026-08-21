namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic ternary operation (x.Alpha ? x.Beta : x.Delta) in a chain of dynamic
/// operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeTernary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public LambdaNodeTernary(LambdaNode left, LambdaNode middle, LambdaNode right) : base()
    {
        DLambdaLeft = left.ThrowWhenNull();
        DLambdaMiddle = middle.ThrowWhenNull();
        DLambdaRight = right.ThrowWhenNull();
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
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
    public override LambdaNodeArgument? GetArgument()
        => DLambdaLeft.GetArgument()
        ?? DLambdaMiddle.GetArgument()
        ?? DLambdaRight.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaLeft { get; }

    /// <summary>
    /// The middle operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaMiddle { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaRight { get; }
}
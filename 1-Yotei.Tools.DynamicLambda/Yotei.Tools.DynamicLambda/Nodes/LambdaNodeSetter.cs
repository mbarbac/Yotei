namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic set operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeSetter : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public LambdaNodeSetter(LambdaNode target, LambdaNode value) : base()
    {
        DLambdaTarget = target.ThrowWhenNull();
        DLambdaValue = value.ThrowWhenNull();
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({DLambdaTarget} = {DLambdaValue})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument()
        => DLambdaTarget.GetArgument()
        ?? DLambdaValue.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaTarget { get; }

    /// <summary>
    /// The value to set the target to.
    /// </summary>
    public LambdaNode DLambdaValue { get; }
}
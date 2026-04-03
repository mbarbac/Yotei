namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic set operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeSetter : DLambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public DLambdaNodeSetter(DLambdaNode target, DLambdaNode value) : base()
    {
        DLambdaTarget = target.ThrowWhenNull();
        DLambdaValue = value.ThrowWhenNull();
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
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
    public override DLambdaNodeArgument? GetArgument()
        => DLambdaTarget.GetArgument()
        ?? DLambdaValue.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaTarget { get; }

    /// <summary>
    /// The value to set the target to.
    /// </summary>
    public DLambdaNode DLambdaValue { get; }
}
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic set operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeSetter : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public LambdaNodeSetter(LambdaNode target, LambdaNode value) : base()
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaValue = value.ThrowWhenNull();

        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaTarget} = {LambdaValue})";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument()
        => LambdaTarget.GetArgument()
        ?? LambdaValue.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }

    /// <summary>
    /// The value to set the target to.
    /// </summary>
    public LambdaNode LambdaValue { get; }
}
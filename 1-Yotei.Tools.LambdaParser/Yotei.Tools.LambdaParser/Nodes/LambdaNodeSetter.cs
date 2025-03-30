namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic unary operation.
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
        LambdaTarget = target.ThrowWhenNull();
        LambdaValue = value.ThrowWhenNull();

        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaTarget} = {LambdaValue})";

    /// <inheritdoc/>
    public override LambdaNodeSetter Clone() => new(
        LambdaTarget.Clone(),
        LambdaValue.Clone());

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
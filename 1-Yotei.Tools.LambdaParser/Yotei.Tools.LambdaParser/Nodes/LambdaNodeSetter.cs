namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic setter operation.
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
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaTarget} = {LambdaValue})";

    /// <inheritdoc/>
    public override LambdaNodeSetter Clone() => new(
        LambdaTarget.Clone(),
        LambdaValue.Clone());

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() =>
        LambdaTarget.GetArgument() ??
        LambdaValue.GetArgument();

    /// <summary>
    /// The target of the set operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }

    /// <summary>
    /// The value of the set operation.
    /// </summary>
    public LambdaNode LambdaValue { get; }
}
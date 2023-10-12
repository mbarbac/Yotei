namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic value setting operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeSetter : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public LambdaNodeSetter(LambdaNode target, LambdaNode value)
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaValue = value.ThrowWhenNull();
        PrintInitialized();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({LambdaTarget} = {LambdaValue})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeSetter Clone() => new(
        LambdaTarget.Clone(),
        LambdaValue.Clone());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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
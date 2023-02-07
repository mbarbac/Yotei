namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a set operation in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeSetter : DynamicNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public DynamicNodeSetter(DynamicNode target, DynamicNode value)
    {
        DynamicTarget = target ?? throw new ArgumentNullException(nameof(target));
        DynamicValue = value ?? throw new ArgumentNullException(nameof(value));
        DebugPrintNew();
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"({DynamicTarget} = {DynamicValue})";

    /// <summary>
    /// The target of the set operation.
    /// </summary>
    public DynamicNode DynamicTarget { get; }

    /// <summary>
    /// The value of the set operation.
    /// </summary>
    public DynamicNode DynamicValue { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public override DynamicNodeArgument? GetArgument() =>
        DynamicTarget.GetArgument() ??
        DynamicValue.GetArgument();
}
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a unary operation in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeUnary : DynamicNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="target"></param>
    public DynamicNodeUnary(ExpressionType operation, DynamicNode target)
    {
        DynamicOperation = operation;
        DynamicTarget = target ?? throw new ArgumentNullException(nameof(target));
        DebugPrintNew();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({DynamicOperation} {DynamicTarget})";

    /// <summary>
    /// The unary operation.
    /// </summary>
    public ExpressionType DynamicOperation { get; }

    /// <summary>
    /// The target operand of the unary operation.
    /// </summary>
    public DynamicNode DynamicTarget { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicNodeArgument? GetArgument() => DynamicTarget.GetArgument();
}
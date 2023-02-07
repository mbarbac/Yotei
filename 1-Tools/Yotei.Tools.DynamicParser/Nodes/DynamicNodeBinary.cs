namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a binary operation in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeBinary : DynamicNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="operation"></param>
    /// <param name="right"></param>
    public DynamicNodeBinary(DynamicNode left, ExpressionType operation, DynamicNode right)
    {
        DynamicLeft = left.ThrowIfNull();
        DynamicOperation = operation;
        DynamicRight = right.ThrowIfNull();
        DebugPrintNew();
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString()
        => $"({DynamicLeft} {DynamicOperation} {DynamicRight})";

    /// <summary>
    /// The left operand of the binary operation.
    /// </summary>
    public DynamicNode DynamicLeft { get; }

    /// <summary>
    /// The binary operation.
    /// </summary>
    public ExpressionType DynamicOperation { get; }

    /// <summary>
    /// The right operand of the binary operation.
    /// </summary>
    public DynamicNode DynamicRight { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public override DynamicNodeArgument? GetArgument() =>
        DynamicLeft.GetArgument() ??
        DynamicRight.GetArgument();
}
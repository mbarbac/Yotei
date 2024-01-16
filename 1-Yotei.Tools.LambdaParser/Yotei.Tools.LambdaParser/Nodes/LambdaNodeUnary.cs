namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic unary operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeUnary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="target"></param>
    public LambdaNodeUnary(ExpressionType operation, LambdaNode target) : base()
    {
        LambdaOperation = operation;
        LambdaTarget = target.ThrowWhenNull();
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaOperation} {LambdaTarget})";

    /// <inheritdoc/>
    public override LambdaNodeUnary Clone() => new(
        LambdaOperation,
        LambdaTarget.Clone());

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaTarget.GetArgument();

    /// <summary>
    /// The unary operation.
    /// </summary>
    public ExpressionType LambdaOperation { get; }

    /// <summary>
    /// The target operand of the unary operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }
}
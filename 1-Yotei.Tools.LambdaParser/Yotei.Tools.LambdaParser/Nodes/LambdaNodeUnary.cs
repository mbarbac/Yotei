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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({LambdaOperation} {LambdaTarget})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeUnary Clone() => new(
        LambdaOperation,
        LambdaTarget.Clone());

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if any.
    /// </summary>
    /// <returns></returns>
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
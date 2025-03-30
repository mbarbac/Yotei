namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic setter operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeUnary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="operation"></param>
    /// <param name="target"></param>
    public LambdaNodeUnary(
        LambdaParser parser,
        ExpressionType operation, LambdaNode target) : base(parser)
    {
        LambdaOperation = operation;
        LambdaTarget = target.ThrowWhenNull();

        LambdaHelpers.Print(LambdaHelpers.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaOperation} {LambdaTarget})";

    /// <inheritdoc/>
    public override LambdaNodeUnary Clone() => new(
        LambdaParser,
        LambdaOperation,
        LambdaTarget.Clone());

    // ----------------------------------------------------

    /// <summary>
    /// The dynamic unary operation represented by this instance.
    /// <br/> The caller is responsable for setting an appropriate value.
    /// </summary>
    public ExpressionType LambdaOperation { get; }

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }
}
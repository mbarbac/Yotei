namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic unary operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeUnary : DLambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="target"></param>
    public DLambdaNodeUnary(ExpressionType operation, DLambdaNode target) : base()
    {
        DLambdaOperation = operation;
        DLambdaTarget = target.ThrowWhenNull();
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"({DLambdaOperation} {DLambdaTarget})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override DLambdaNodeArgument? GetArgument() => DLambdaTarget.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The dynamic unary operation represented by this instance.
    /// <br/> The caller is responsable for setting an appropriate value.
    /// </summary>
    public ExpressionType DLambdaOperation { get; }

    /// <summary>
    /// The target of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaTarget { get; }
}
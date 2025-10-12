namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic unary operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeUnary : LambdaNode
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

        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({LambdaOperation} {LambdaTarget})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override LambdaNodeArgument? GetArgument() => LambdaTarget.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The dynamic unary operation represented by this instance.
    /// <br/> The caller is responsable for setting an appropriate value.
    /// </summary>
    public ExpressionType LambdaOperation { get; }

    /// <summary>
    /// The target of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }
}
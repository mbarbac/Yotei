namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic unary operation.
/// <para>Instances of this class are intended to be immutable ones.</para>
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

        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaOperation} {LambdaTarget})";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaTarget.GetArgument();

    /// <inheritdoc/>
    public override LambdaNodeUnary Clone() => new(
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
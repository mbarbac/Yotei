namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic cast or convert operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeConvert : DLambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public DLambdaNodeConvert(Type type, DLambdaNode target) : base()
    {
        DLambdaType = type.ThrowWhenNull();
        DLambdaTarget = target.ThrowWhenNull();
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"(({DLambdaType.EasyName()}) {DLambdaTarget})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override DLambdaNodeArgument? GetArgument() => DLambdaTarget.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The type to convert the target to.
    /// </summary>
    public Type DLambdaType { get; }

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public DLambdaNode DLambdaTarget { get; }
}
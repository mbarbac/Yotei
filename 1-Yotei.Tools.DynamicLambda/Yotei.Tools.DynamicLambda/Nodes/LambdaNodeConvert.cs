namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic cast or convert operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeConvert : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public LambdaNodeConvert(Type type, LambdaNode target) : base()
    {
        DLambdaType = type.ThrowWhenNull();
        DLambdaTarget = target.ThrowWhenNull();
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
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
    public override LambdaNodeArgument? GetArgument() => DLambdaTarget.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The type to convert the target to.
    /// </summary>
    public Type DLambdaType { get; }

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode DLambdaTarget { get; }
}
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic type conversion operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeConvert : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="type"></param>
    public LambdaNodeConvert(LambdaNode target, Type type) : base()
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaType = type.ThrowWhenNull();
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"(({LambdaType.EasyName()}) {LambdaTarget})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeConvert Clone() => new(
        LambdaTarget.Clone(),
        LambdaType);

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if any.
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => LambdaTarget.GetArgument();

    /// <summary>
    /// The target operand of the conversion operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }

    /// <summary>
    /// The type the target is requested to be converted to.
    /// </summary>
    public Type LambdaType { get; }
}
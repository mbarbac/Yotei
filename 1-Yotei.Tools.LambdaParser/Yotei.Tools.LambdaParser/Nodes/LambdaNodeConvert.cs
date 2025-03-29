namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic binary operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeConvert : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public LambdaNodeConvert(
        LambdaParser parser,
        Type type, LambdaNode target) : base(parser)
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaType = type.ThrowWhenNull();

        LambdaDebug.Print(LambdaDebug.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"(({LambdaType.EasyName()}) {LambdaTarget})";

    /// <inheritdoc/>
    public override LambdaNodeConvert Clone() => new(
        LambdaParser,
        LambdaType,
        LambdaTarget.Clone());

    // ----------------------------------------------------

    /// <summary>
    /// The type to convert the target to.
    /// </summary>
    public Type LambdaType { get; }

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }
}
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic conversion operation.
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
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"(({LambdaType.EasyName()}) {LambdaTarget})";

    /// <inheritdoc/>
    public override LambdaNodeConvert Clone() => new(
        LambdaTarget.Clone(),
        LambdaType);

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaTarget.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }

    /// <summary>
    /// The type to convert the target to.
    /// </summary>
    public Type LambdaType { get; }
}
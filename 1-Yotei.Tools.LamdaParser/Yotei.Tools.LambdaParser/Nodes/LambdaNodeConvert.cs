namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic convert or cast operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeConvert : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public LambdaNodeConvert(Type type, LambdaNode target) : base()
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaType = type.ThrowWhenNull();

        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"(({LambdaType.EasyName()}) {LambdaTarget})";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaTarget.GetArgument();

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
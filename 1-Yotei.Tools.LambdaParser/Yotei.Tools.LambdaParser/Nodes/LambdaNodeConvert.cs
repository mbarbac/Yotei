namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic convert or cast operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeConvert : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="type"></param>
    public LambdaNodeConvert(LambdaNode target, Type type)
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaType = type.ThrowWhenNull();
        PrintInitialized();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"(({LambdaType.EasyName()}) {LambdaTarget})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeConvert Clone() => new(
        LambdaTarget.Clone(),
        LambdaType);

    /// <summary>
    /// <inheritdoc/>
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
namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a dynamic cast or convert operation in a chain of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
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
        LambdaType = type.ThrowWhenNull();
        LambdaTarget = target.ThrowWhenNull();
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    // HIGH: EasyName related...
    //public override string ToString() => $"(({LambdaType.EasyName()}) {LambdaTarget})";
    public override string ToString() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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
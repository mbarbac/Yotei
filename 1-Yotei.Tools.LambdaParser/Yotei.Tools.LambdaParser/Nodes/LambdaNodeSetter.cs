namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a dynamic set operation in a chain of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeSetter : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public LambdaNodeSetter(LambdaNode target, LambdaNode value) : base()
    {
        LambdaTarget = target.ThrowWhenNull();
        LambdaValue = value.ThrowWhenNull();
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({LambdaTarget} = {LambdaValue})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument()
        => LambdaTarget.GetArgument()
        ?? LambdaValue.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The target operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaTarget { get; }

    /// <summary>
    /// The value to set the target to.
    /// </summary>
    public LambdaNode LambdaValue { get; }
}
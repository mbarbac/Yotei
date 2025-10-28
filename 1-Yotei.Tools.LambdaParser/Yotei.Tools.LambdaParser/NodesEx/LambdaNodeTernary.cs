namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a dynamic ternary operation ('x.Alpha ? x.Beta : x.Delta') in a chain of dynamic
/// operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeTernary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public LambdaNodeTernary(LambdaNode left, LambdaNode middle, LambdaNode right) : base()
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaMiddle = middle.ThrowWhenNull();
        LambdaRight = right.ThrowWhenNull();
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({LambdaLeft} ? {LambdaMiddle} : {LambdaRight})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument()
        => LambdaLeft.GetArgument()
        ?? LambdaMiddle.GetArgument()
        ?? LambdaRight.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaLeft { get; }

    /// <summary>
    /// The middle operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaMiddle { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaRight { get; }
}
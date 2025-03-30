namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic ternary operation, as in 'x.Alpha ? x.Beta : x.Delta'.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeTernary : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="left"></param>
    /// <param name="middle"></param>
    /// <param name="right"></param>
    public LambdaNodeTernary(
        LambdaParser parser,
        LambdaNode left, LambdaNode middle, LambdaNode right) : base(parser)
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaMiddle = middle.ThrowWhenNull();
        LambdaRight = right.ThrowWhenNull();

        LambdaHelpers.Print(LambdaHelpers.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaLeft} ? {LambdaMiddle} : {LambdaRight})";

    /// <inheritdoc/>
    public override LambdaNodeTernary Clone() => new(
        LambdaParser,
        LambdaLeft.Clone(),
        LambdaMiddle.Clone(),
        LambdaRight.Clone());

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
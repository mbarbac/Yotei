namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic coalesce operation, as in 'x.Alpha ?? x.Beta'.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeCoalesce : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public LambdaNodeCoalesce(
        LambdaParser parser,
        LambdaNode left, LambdaNode right) : base(parser)
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaRight = right.ThrowWhenNull();

        LambdaDebug.Print(LambdaDebug.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaLeft} ?? {LambdaRight})";

    /// <inheritdoc/>
    public override LambdaNodeCoalesce Clone() => new(
        LambdaParser,
        LambdaLeft.Clone(),
        LambdaRight.Clone());

    // ----------------------------------------------------

    /// <summary>
    /// The left operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaLeft { get; }

    /// <summary>
    /// The right operand of the dynamic operation.
    /// </summary>
    public LambdaNode LambdaRight { get; }
}
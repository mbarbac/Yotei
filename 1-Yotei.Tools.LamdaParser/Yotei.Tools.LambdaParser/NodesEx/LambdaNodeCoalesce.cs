namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic coalesce operation (as in 'x.Alpha ?? x.Beta').
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeCoalesce : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public LambdaNodeCoalesce(LambdaNode left, LambdaNode right) : base()
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaRight = right.ThrowWhenNull();

        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaLeft} ?? {LambdaRight})";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument()
        => LambdaLeft.GetArgument()
        ?? LambdaRight.GetArgument();

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
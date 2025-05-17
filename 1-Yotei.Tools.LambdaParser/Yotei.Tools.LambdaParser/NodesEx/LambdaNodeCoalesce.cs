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
    /// <param name="left"></param>
    /// <param name="right"></param>
    public LambdaNodeCoalesce(LambdaNode left, LambdaNode right) : base()
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaRight = right.ThrowWhenNull();

        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"({LambdaLeft} ?? {LambdaRight})";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument()
        => LambdaLeft.GetArgument()
        ?? LambdaRight.GetArgument();

    /// <inheritdoc/>
    public override LambdaNodeCoalesce Clone() => new(
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
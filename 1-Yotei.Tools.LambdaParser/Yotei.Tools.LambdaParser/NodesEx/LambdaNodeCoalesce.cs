namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a dynamic coalesce operation ('x.Alpha ?? x.Beta') in a chain of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
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
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"({LambdaLeft} ?? {LambdaRight})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
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
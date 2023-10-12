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
    public LambdaNodeCoalesce(LambdaNode left, LambdaNode right)
    {
        LambdaLeft = left.ThrowWhenNull();
        LambdaRight = right.ThrowWhenNull();
        PrintInitialized();
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
    public override LambdaNodeCoalesce Clone() => new(
        LambdaLeft.Clone(),
        LambdaRight.Clone());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() =>
        LambdaLeft.GetArgument() ??
        LambdaRight.GetArgument();

    /// <summary>
    /// The left operand of the coalesce operation.
    /// </summary>
    public LambdaNode LambdaLeft { get; }

    /// <summary>
    /// The right operand of the coalesce operation.
    /// </summary>
    public LambdaNode LambdaRight { get; }
}
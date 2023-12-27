namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a constant value in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeConstant : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public LambdaNodeConstant(object? value) : base()
    {
        if (value is LambdaNode)
            throw new ArgumentException(
                $"Cannot use a '{nameof(LambdaNode)}' as the value of a constant node.")
                .WithData(value);

        LambdaValue = value;
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => LambdaValue == null
        ? "'NULL'"
        : $"'{LambdaValue.Sketch()}'";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeConstant Clone() => new(LambdaValue.TryClone());

    /// <summary>
    /// Returns the dynamic argument ultimately associated with this instance, or null if any.
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => null;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public object? LambdaValue { get; }
}
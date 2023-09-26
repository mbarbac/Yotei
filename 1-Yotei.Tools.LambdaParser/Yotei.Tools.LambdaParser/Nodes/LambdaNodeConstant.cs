namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a contant in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeConstant : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public LambdaNodeConstant(object? value)
    {
        if (value is LambdaNode)
            throw new ArgumentException(
                $"Cannot use a '{nameof(LambdaNode)}' as the value of a constant node.")
                .WithData(value);

        LambdaValue = value;
        PrintInitialized();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => LambdaValue == null ? "'NULL'" : $"'{LambdaValue.Sketch()}'";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeConstant Clone() => new(LambdaValue.TryClone());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => null;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public object? LambdaValue { get; }
}
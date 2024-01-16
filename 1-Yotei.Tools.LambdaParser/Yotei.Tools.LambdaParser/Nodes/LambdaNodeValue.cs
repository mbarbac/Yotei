namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a constant value in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeValue : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public LambdaNodeValue(object? value) : base()
    {
        if (value is LambdaNode)
            throw new ArgumentException(
                $"Cannot use a '{nameof(LambdaNode)}' as the value of a constant node.")
                .WithData(value);

        LambdaValue = value;
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => LambdaValue switch
    {
        null => "'NULL'",
        _ => $"'{LambdaValue.Sketch()}'"
    };

    /// <inheritdoc/>
    public override LambdaNodeValue Clone() => new(LambdaValue.TryClone());

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => null;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public object? LambdaValue { get; }
}
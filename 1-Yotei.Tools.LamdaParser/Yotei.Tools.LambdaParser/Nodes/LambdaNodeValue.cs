namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a constant value in a chain of dynamic operations.
/// <br/> Instances of this type are typically captured from constant values or from the returned
/// ones of methods or functions invoked while parsing dynamic lambda expressions.
/// <br/> Instances of this type are intended to be immutable ones, but the carried value may
/// itself be mutable.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeValue : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public LambdaNodeValue(object? value) : base()
    {
        LambdaValue = value switch
        {
            LambdaNode => throw new ArgumentException(
                "Cannot use a LambdaNode as the value carried by a valued instance.")
                .WithData(value),

            _ => value,
        };
        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"'{LambdaValue.Sketch()}'";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaValue is LambdaNode node
        ? node.GetArgument()
        : null;

    // ----------------------------------------------------

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? LambdaValue { get; }
}
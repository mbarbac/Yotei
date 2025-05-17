namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a constant value in a chain of dynamic operations.
/// <br/> Instances of this type are also captured containig the returned value of methods or
/// functions invoked while parsing the dynamic expression.
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

        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"'{LambdaValue.Sketch()}'";

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaValue is LambdaNode node
        ? node.GetArgument()
        : null;

    /// <inheritdoc/>
    public override LambdaNodeValue Clone() => new(LambdaValue.TryClone());

    // ----------------------------------------------------

    /// <summary>
    /// The value carried by this instance.
    /// </summary>
    public object? LambdaValue { get; }
}
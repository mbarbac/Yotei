namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a constant arbitrary value in a chain of dynamic operations. The value may also have
/// been obtained from the result of invoking an existing regular method that was used in the dynamic
/// lambda expression.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeValue : DLambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DLambdaNodeValue(object? value)
    {
        if (value is DLambdaNode) throw new ArgumentException(
            "Cannot use a lambda node as the value carried by this instance.")
            .WithData(value);
        
        DLambdaValue = value;
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"'{DLambdaValue.Sketch()}'";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override DLambdaNodeArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? DLambdaValue { get; }
}
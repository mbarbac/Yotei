namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a constant arbitrary value in a chain of dynamic operations. The value may also have
/// been obtained from the result of invoking an existing regular method that was used in the dynamic
/// lambda expression.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeValue : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public LambdaNodeValue(object? value)
    {
        if (value is LambdaNode) throw new ArgumentException(
            "Cannot use a lambda node as the value carried by this instance.")
            .WithData(value);
        
        DLambdaValue = value;
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
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
    public override LambdaNodeArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? DLambdaValue { get; }
}
namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a constant value in a chain of dynamic operations. Instances of this class also
/// represent the result of the invocation of external methods.
/// <br/> Instances of this type are intended to be immutable ones.
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
        LambdaValue = value switch
        {
            LambdaNode => throw new ArgumentException(
                "Cannot use a LambdaNode as the value carried by a value-alike instance.")
                .WithData(value),

            _ => value,
        };
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var options = SketchOptions.Default with { TypeOptions = null };
        var str = $"'{LambdaValue.Sketch(options)}'";
        return str;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => null;

    // ----------------------------------------------------

    /// <summary>
    /// The actual value carried by this instance.
    /// </summary>
    public object? LambdaValue { get; }
}
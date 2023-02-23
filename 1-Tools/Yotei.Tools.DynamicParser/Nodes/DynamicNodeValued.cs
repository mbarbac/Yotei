namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a value that is referenced in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeValued : DynamicNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public DynamicNodeValued(object? value)
    {
        if (value is DynamicNode)
            throw new ArgumentException(
                "Cannot use a dynamic node as the value of a DynamicNodeValued instance.")
                .WithData(value);

        DynamicValue = value;
        DebugPrintNew();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => DynamicValue == null
        ? "'NULL'"
        : $"'{DynamicValue.Sketch()}'";

    /// <summary>
    /// The constant value carried by this instance.
    /// </summary>
    public object? DynamicValue { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicNodeArgument? GetArgument() => null;
}
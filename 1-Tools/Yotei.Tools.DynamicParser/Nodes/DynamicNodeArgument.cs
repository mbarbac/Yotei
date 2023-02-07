namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the dynamic argument in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeArgument : DynamicNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    public DynamicNodeArgument(string name) : base()
    {
        DynamicName = ValidateDynamicName(name);
        DebugPrintNew();
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => DynamicName;

    /// <summary>
    /// The name of the dynamic argument.
    /// </summary>
    public string DynamicName { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public override DynamicNodeArgument? GetArgument() => this;
}
namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a host invocation in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeInvoked : DynamicNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="arguments"></param>
    public DynamicNodeInvoked(DynamicNode host, IEnumerable<DynamicNode> arguments) : base(host)
    {
        DynamicArguments = ValidateDynamicArguments(arguments, canbeEmpty: true);
        DebugPrintNew();
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString()
        => $"{DynamicHost}({string.Join(", ", DynamicArguments.Select(x => x.ToString()))})";

    /// <summary>
    /// The arguments to use for the method invocation. This collection may be an empty one.
    /// </summary>
    public IImmutableList<DynamicNode> DynamicArguments { get; }

    /// <inheritdoc>
    /// </inheritdoc>
    public override DynamicNodeArgument? GetArgument() => DynamicHost.GetArgument();
}
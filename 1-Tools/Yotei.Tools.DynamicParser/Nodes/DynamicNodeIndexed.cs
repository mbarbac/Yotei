namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an indexed getter in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeIndexed : DynamicNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public DynamicNodeIndexed(DynamicNode host, IEnumerable<DynamicNode> indexes) : base(host)
    {
        DynamicIndexes = ValidateDynamicArguments(indexes, canbeEmpty: false);
        DebugPrintNew();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => $"{DynamicHost}[{string.Join(", ", DynamicIndexes.Select(x => x.ToString()))}]";

    /// <summary>
    /// The indexes to use for the indexed getter. This collection is not an empty one.
    /// </summary>
    public IImmutableList<DynamicNode> DynamicIndexes { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicNodeArgument? GetArgument() => DynamicHost.GetArgument();
}
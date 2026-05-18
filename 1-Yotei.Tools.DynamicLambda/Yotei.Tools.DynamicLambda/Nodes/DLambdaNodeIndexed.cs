namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a hosted dynamic indexed get operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeIndexed : DLambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public DLambdaNodeIndexed(DLambdaNode host, IEnumerable<DLambdaNode> indexes) : base(host)
    {
        DLambdaIndexes = DLambdaParser.ValidateArguments(indexes, canBeEmpty: false);
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{DLambdaHost}[{string.Join(", ", DLambdaIndexes.Select(static x => x.ToString()))}]";

    // ----------------------------------------------------

    /// <summary>
    /// The collection of indexes of the indexed get operation, which cannot be an empty one.
    /// </summary>
    public ImmutableArray<DLambdaNode> DLambdaIndexes { get; }
}
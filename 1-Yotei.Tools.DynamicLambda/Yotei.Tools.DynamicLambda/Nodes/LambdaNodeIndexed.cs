namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a hosted dynamic indexed get operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeIndexed : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeIndexed(LambdaNode host, IEnumerable<LambdaNode> indexes) : base(host)
    {
        LambdaIndexes = LambdaParser.ValidateArguments(indexes, canBeEmpty: false);
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(static x => x.ToString()))}]";

    // ----------------------------------------------------

    /// <summary>
    /// The collection of indexes of the indexed get operation, which cannot be an empty one.
    /// </summary>
    public ImmutableArray<LambdaNode> LambdaIndexes { get; }
}
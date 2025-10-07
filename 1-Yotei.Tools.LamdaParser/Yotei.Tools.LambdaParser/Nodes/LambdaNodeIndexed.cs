namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic indexed get operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeIndexed : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeIndexed(LambdaNode host, IEnumerable<LambdaNode> indexes) : base(host)
    {
        LambdaIndexes = ValidateLambdaArguments(indexes, canbeEmpty: false);
        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(x => x.ToString()))}]";
        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The not null and not empty collection of indexes used for the indexed get operation.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaIndexes { get; }
}
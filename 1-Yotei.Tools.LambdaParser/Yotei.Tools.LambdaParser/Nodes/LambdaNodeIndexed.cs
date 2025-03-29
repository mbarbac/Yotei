namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic get indexed operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeIndexed : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeIndexed(
        LambdaParser parser,
        LambdaNode host, IEnumerable<LambdaNode> indexes) : base(parser, host)
    {
        LambdaIndexes = ValidateLambdaArguments(indexes, canBeEmpty: false);

        LambdaDebug.Print(LambdaDebug.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(x => x.ToString()))}]";
        return str;
    }

    /// <inheritdoc/>
    public override LambdaNodeIndexed Clone() => new(
        LambdaParser,
        LambdaHost.Clone(),
        LambdaIndexes.Select(x => x.Clone()).ToImmutableList());

    // ----------------------------------------------------

    /// <summary>
    /// The not null and not empty collection of indexes used for the indexed get operation.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaIndexes { get; }
}
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
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeIndexed(LambdaNode host, IEnumerable<LambdaNode> indexes) : base(host)
    {
        LambdaIndexes = ValidateLambdaArguments(indexes, canBeEmpty: false);
        LambdaParser.Print($"- New: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString()
         => $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(x => x.ToString()))}]";

    /// <inheritdoc/>
    public override LambdaNodeIndexed Clone() => new(
        LambdaHost.Clone(),
        LambdaIndexes.Select(x => x.Clone()).ToImmutableList());

    // ----------------------------------------------------

    /// <summary>
    /// The not null and not empty collection of indexes used for the indexed get operation.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaIndexes { get; }
}
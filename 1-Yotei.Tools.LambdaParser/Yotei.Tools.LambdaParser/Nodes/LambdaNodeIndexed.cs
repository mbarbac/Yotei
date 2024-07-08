namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic indexed get operation.
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
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(x => x.ToString()))}]";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeIndexed Clone() => new(
        LambdaHost.Clone(),
        LambdaIndexes.Select(x => x.Clone()).ToImmutableList());

    /// <summary>
    /// The indexes to use for the indexed get operation, which is not null and not empty.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaIndexes { get; }
}
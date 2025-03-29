namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic host invocation operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeInvoke : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeInvoke(
        LambdaParser parser,
        LambdaNode host, IEnumerable<LambdaNode> indexes) : base(parser, host)
    {
        LambdaArguments = ValidateLambdaArguments(indexes, canBeEmpty: true);

        LambdaDebug.Print(LambdaDebug.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{LambdaHost}({string.Join(", ", LambdaArguments.Select(x => x.ToString()))})";

    /// <inheritdoc/>
    public override LambdaNodeInvoke Clone() => new(
        LambdaParser,
        LambdaHost.Clone(),
        LambdaArguments.Select(x => x.Clone()).ToImmutableList());

    // ----------------------------------------------------

    /// <summary>
    /// The collection of arguments used for the host invocation, or an empty list if any.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}
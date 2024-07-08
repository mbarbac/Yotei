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
    /// <param name="host"></param>
    /// <param name="arguments"></param>
    public LambdaNodeInvoke(LambdaNode host, IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaArguments = ValidateLambdaArguments(arguments, canBeEmpty: true);
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{LambdaHost}({string.Join(", ", LambdaArguments.Select(x => x.ToString()))})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeInvoke Clone() => new(
        LambdaHost.Clone(),
        LambdaArguments.Select(x => x.Clone()).ToImmutableList());

    /// <summary>
    /// The arguments to use for the host invocation, or an empty list if any.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}
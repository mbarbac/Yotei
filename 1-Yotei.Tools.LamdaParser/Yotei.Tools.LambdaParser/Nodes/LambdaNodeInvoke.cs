namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic host invocation operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeInvoke : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="arguments"></param>
    public LambdaNodeInvoke(LambdaNode host, IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaArguments = ValidateLambdaArguments(arguments, canbeEmpty: true);
        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => $"{LambdaHost}({string.Join(", ", LambdaArguments.Select(x => x.ToString()))})";

    // ----------------------------------------------------

    /// <summary>
    /// The collection of arguments used for the host invocation operation, which can be an
    /// empty one when needed.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}
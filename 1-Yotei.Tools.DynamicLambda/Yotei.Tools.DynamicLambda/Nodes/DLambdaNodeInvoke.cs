namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a dynamic host invocation operation in a chain of dynamic operations. Instances of
/// this type represent rounded bracket 'x(...)' operation on dynamic hosts.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeInvoke : DLambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="arguments"></param>
    public DLambdaNodeInvoke(DLambdaNode host, IEnumerable<DLambdaNode> arguments) : base(host)
    {
        DLambdaArguments = DLambdaParser.ValidateArguments(arguments, canBeEmpty: true);
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{DLambdaHost}({string.Join(", ", DLambdaArguments.Select(x => x.ToString()))})";

    // ----------------------------------------------------

    /// <summary>
    /// The collection of arguments used for the host invocation operation, which can be an empty
    /// one when needed.
    /// </summary>
    public ImmutableArray<DLambdaNode> DLambdaArguments { get; }
}
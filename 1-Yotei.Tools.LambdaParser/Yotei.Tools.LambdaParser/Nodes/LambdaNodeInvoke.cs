namespace Yotei.Tools.LambdaParser;

// ========================================================
/// <summary>
/// Represent a dynamic host invocation operation in a chain of dynamic operations. Instances
/// of this class represent rounded brackets '(...)' operations on dynamic hosts.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public class LambdaNodeInvoke : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="arguments"></param>
    public LambdaNodeInvoke(LambdaNode host, IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaArguments = LambdaParser.ValidateArguments(arguments, canBeEmpty: true);
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
        => $"{LambdaHost}({string.Join(", ", LambdaArguments.Select(x => x.ToString()))})";

    // ----------------------------------------------------

    /// <summary>
    /// The collection of arguments used for the host invocation operation, which can be an empty
    /// one when needed.
    /// </summary>
    public ImmutableArray<LambdaNode> LambdaArguments { get; }
}
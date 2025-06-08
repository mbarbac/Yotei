namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic host invocation operation.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeInvoke : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="arguments"></param>
    public LambdaNodeInvoke(
        LambdaNode host, IEnumerable<LambdaNode> arguments) : base(host)
    {
        LambdaArguments = LambdaHelpers.ValidateLambdaArguments(arguments, canBeEmpty: true);
        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{LambdaHost}({string.Join(", ", LambdaArguments.Select(x => x.ToString()))})";

    /// <inheritdoc/>
    public override LambdaNodeInvoke Clone() => new(
        LambdaHost.Clone(),
        LambdaArguments.Select(x => x.Clone()).ToImmutableList());

    // ----------------------------------------------------

    /// <summary>
    /// The collection of arguments used for the host invocation, or an empty list if any.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaArguments { get; }
}
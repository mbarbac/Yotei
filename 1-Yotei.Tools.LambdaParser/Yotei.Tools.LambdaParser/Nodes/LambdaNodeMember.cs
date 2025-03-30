namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic get member operation.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeMember : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public LambdaNodeMember(
        LambdaParser parser, LambdaNode host, string name) : base(parser, host)
    {
        LambdaName = LambdaHelpers.ValidateName(name);

        LambdaHelpers.Print(LambdaHelpers.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"{LambdaHost}.{LambdaName}";

    /// <inheritdoc/>
    public override LambdaNodeMember Clone() => new(
        LambdaParser,
        LambdaHost.Clone(),
        LambdaName);

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string LambdaName { get; }
}
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
    /// <param name="host"></param>
    /// <param name="name"></param>
    public LambdaNodeMember(LambdaNode host, string name) : base(host)
    {
        LambdaName = ValidateName(name);
        LambdaParser.Print(LambdaParser.NewNodeColor, $"- New Node: {ToDebugString()}");
    }

    /// <inheritdoc/>
    public override string ToString() => $"{LambdaHost}.{LambdaName}";

    /// <inheritdoc/>
    public override LambdaNodeMember Clone() => new(
        LambdaHost.Clone(),
        LambdaName);

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string LambdaName { get; }
}
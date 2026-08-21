namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a hosted dynamic get operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
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
        LambdaName = LambdaParser.ValidateName(name);
        LambdaParser.ToDebug(LambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{LambdaHost}.{LambdaName}";

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string LambdaName { get; }
}
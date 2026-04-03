namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a hosted dynamic get operation in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DLambdaNodeMember : DLambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public DLambdaNodeMember(DLambdaNode host, string name) : base(host)
    {
        DLambdaName = DLambdaParser.ValidateName(name);
        DLambdaParser.ToDebug(DLambdaParser.NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{DLambdaHost}.{DLambdaName}";

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string DLambdaName { get; }
}
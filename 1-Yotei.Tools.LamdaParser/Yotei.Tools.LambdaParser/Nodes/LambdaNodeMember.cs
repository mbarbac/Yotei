namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic get operation.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public sealed class LambdaNodeMember : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public LambdaNodeMember(LambdaNode host, string name) : base(host)
    {
        LambdaName = ValidateLambdaName(name);
        LambdaParser.Print(NewNodeColor, $"- NODE new: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{LambdaHost}.{LambdaName}";

    // ----------------------------------------------------

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string LambdaName { get; }
}
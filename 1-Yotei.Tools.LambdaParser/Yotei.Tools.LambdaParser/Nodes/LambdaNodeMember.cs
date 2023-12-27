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
        LambdaParser.Print(this, $"- New: {ToDebugString()}");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{LambdaHost}.{LambdaName}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeMember Clone() => new(
        LambdaHost.Clone(),
        LambdaName);

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string LambdaName { get; }
}
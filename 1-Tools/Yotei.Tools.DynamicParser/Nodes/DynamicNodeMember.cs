namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a member getter in a dynamic lambda expression.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class DynamicNodeMember : DynamicNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name"></param>
    public DynamicNodeMember(DynamicNode host, string name) : base(host)
    {
        DynamicName = ValidateDynamicName(name);
        DebugPrintNew();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{DynamicHost}.{DynamicName}";

    /// <summary>
    /// The name of the dynamic member.
    /// </summary>
    public string DynamicName { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override DynamicNodeArgument? GetArgument() => DynamicHost.GetArgument();
}
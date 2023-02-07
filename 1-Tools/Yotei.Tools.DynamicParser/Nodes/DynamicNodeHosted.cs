namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic node that depends on a given host.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class DynamicNodeHosted : DynamicNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public DynamicNodeHosted(DynamicNode host)
        => DynamicHost = host ?? throw new ArgumentNullException(nameof(host));

    /// <summary>
    /// The host this instance depends on.
    /// </summary>
    public DynamicNode DynamicHost { get; }
}
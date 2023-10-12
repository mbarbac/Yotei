namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a hosted node in a chain of dynamic operations, that is, a node that is hosted
/// in a previous one.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNodeHosted : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public LambdaNodeHosted(LambdaNode host) => LambdaHost = host.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => LambdaHost.GetArgument();

    /// <summary>
    /// The host this instance depends on.
    /// </summary>
    public LambdaNode LambdaHost { get; }
}
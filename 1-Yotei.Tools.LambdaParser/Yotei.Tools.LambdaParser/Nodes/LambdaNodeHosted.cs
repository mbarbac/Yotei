namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a hosted node in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNodeHosted : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="host"></param>
    public LambdaNodeHosted(
        LambdaParser parser,
        LambdaNode host) : base(parser) => LambdaHost = host.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// The host this instance depends on.
    /// </summary>
    public LambdaNode LambdaHost { get; }
}
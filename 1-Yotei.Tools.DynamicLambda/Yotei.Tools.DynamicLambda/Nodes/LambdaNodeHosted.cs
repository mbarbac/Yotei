namespace Yotei.Tools.DynamicLambda;

// ========================================================
/// <summary>
/// Represents a hosted node in a chain of dynamic operations.
/// <br/> Instances of this type are immutable ones.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNodeHosted : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public LambdaNodeHosted(LambdaNode host) : base() => DLambdaHost = host.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override LambdaNodeArgument? GetArgument() => DLambdaHost.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The host this instance depends on.
    /// </summary>
    public LambdaNode DLambdaHost { get; }
}
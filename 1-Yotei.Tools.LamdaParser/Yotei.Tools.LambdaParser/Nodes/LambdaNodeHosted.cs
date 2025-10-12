namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a hosted node in a tree of dynamic operations.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract class LambdaNodeHosted : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public LambdaNodeHosted(LambdaNode host) : base() => LambdaHost = host.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override LambdaNodeArgument? GetArgument() => LambdaHost.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The host this instance depends on.
    /// </summary>
    public LambdaNode LambdaHost { get; }
}
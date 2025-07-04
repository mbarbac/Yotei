﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a hosted node in a chain of dynamic operations.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNodeHosted : LambdaNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    public LambdaNodeHosted(LambdaNode host) : base() => LambdaHost = host.ThrowWhenNull();

    /// <inheritdoc/>
    public override LambdaNodeArgument? GetArgument() => LambdaHost.GetArgument();

    // ----------------------------------------------------

    /// <summary>
    /// The host this instance depends on.
    /// </summary>
    public LambdaNode LambdaHost { get; }
}
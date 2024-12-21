namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a node in a chain of dynamic operations.
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public abstract class LambdaNode : DynamicObject, ICloneable
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    public abstract LambdaNode Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Returns a debug representation of this instance.
    /// </summary>
    internal string ToDebugString()
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The unique ID of this instance.
    /// </summary>
    public long LambdaId { get; }
    static long LastLambdaId = 0;

    /// <summary>
    /// The current version of this instance.
    /// </summary>
    public long LambdaVersion { get; internal set; }
    static long LastLambdaVersion = 0;
}
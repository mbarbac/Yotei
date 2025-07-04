﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a dynamic get indexed operation.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[DebuggerDisplay("{ToDebugString()}")]
public class LambdaNodeIndexed : LambdaNodeHosted
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="indexes"></param>
    public LambdaNodeIndexed(
        LambdaNode host, IEnumerable<LambdaNode> indexes) : base(host)
    {
        LambdaIndexes = LambdaHelpers.ValidateLambdaArguments(indexes, canBeEmpty: false);

        LambdaHelpers.PrintNode(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var str = $"{LambdaHost}[{string.Join(", ", LambdaIndexes.Select(x => x.ToString()))}]";
        return str;
    }

    /// <inheritdoc/>
    public override LambdaNodeIndexed Clone() => new(
        LambdaHost.Clone(),
        LambdaIndexes.Select(x => x.Clone()).ToImmutableList());

    // ----------------------------------------------------

    /// <summary>
    /// The not null and not empty collection of indexes used for the indexed get operation.
    /// </summary>
    public IImmutableList<LambdaNode> LambdaIndexes { get; }
}
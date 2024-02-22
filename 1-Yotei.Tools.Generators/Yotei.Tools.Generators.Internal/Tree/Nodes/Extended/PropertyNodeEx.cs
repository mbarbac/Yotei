namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class PropertyNodeEx(TypeNode parent, PropertyCandidate candidate)
    : PropertyNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public PropertyCandidate Candidate { get; } = candidate.ThrowWhenNull();
}
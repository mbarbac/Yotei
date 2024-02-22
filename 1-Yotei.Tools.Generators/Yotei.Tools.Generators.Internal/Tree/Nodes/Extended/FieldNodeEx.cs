namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="FieldNode"/>
internal class FieldNodeEx(TypeNode parent, FieldCandidate candidate)
    : FieldNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public FieldCandidate Candidate { get; } = candidate.ThrowWhenNull();
}
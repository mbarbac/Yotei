namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class TypeNodeEx(INode parent, TypeCandidate candidate)
    : TypeNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public TypeCandidate Candidate { get; } = candidate.ThrowWhenNull();
}
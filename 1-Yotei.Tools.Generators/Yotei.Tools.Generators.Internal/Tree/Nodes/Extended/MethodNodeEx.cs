namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="MethodNode"/>
internal class MethodNodeEx(TypeNode parent, MethodCandidate candidate)
    : MethodNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public MethodCandidate Candidate { get; } = candidate.ThrowWhenNull();
}
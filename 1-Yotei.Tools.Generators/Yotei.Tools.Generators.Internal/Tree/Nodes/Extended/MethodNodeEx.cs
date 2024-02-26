namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="MethodNode"/>
internal class MethodNodeEx(TypeNode parent, MethodCandidate candidate)
    : MethodNode(parent, candidate.Symbol)
{
    public MethodCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public MethodDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
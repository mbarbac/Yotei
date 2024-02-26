namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class TypeNodeEx(INode parent, TypeCandidate candidate)
    : TypeNode(parent, candidate.Symbol)
{
    public TypeCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public TypeDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
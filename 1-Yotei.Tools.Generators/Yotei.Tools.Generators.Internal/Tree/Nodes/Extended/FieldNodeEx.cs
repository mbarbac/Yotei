namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="FieldNode"/>
internal class FieldNodeEx(TypeNode parent, FieldCandidate candidate)
    : FieldNode(parent, candidate.Symbol)
{
    public FieldCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public FieldDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
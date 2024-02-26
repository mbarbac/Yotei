namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class PropertyNodeEx(TypeNode parent, PropertyCandidate candidate)
    : PropertyNode(parent, candidate.Symbol)
{
    public PropertyCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public PropertyDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
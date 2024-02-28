namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field node in the source code generation hierarchy built from a given
/// candidate.
/// </summary>
/// <param name="parent"></param>
/// <param name="candidate"></param>
internal class FieldNodeEx(
    TypeNode parent, FieldCandidate candidate) : FieldNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public FieldCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public FieldDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// The semantic model this instance is associated with.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
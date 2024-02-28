namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type node in the source code generation hierarchy built from a given candidate.
/// </summary>
/// <param name="parent"></param>
/// <param name="candidate"></param>
internal class TypeNodeEx(
    INode parent, TypeCandidate candidate) : TypeNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public TypeCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public TypeDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// The semantic model this instance is associated with.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
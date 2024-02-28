namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a property node in the source code generation hierarchy built from a given
/// candidate.
/// </summary>
/// <param name="parent"></param>
/// <param name="candidate"></param>
internal class PropertyNodeEx(
    TypeNode parent, PropertyCandidate candidate) : PropertyNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public PropertyCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public PropertyDeclarationSyntax Syntax => Candidate.Syntax;

    /// <summary>
    /// The semantic model this instance is associated with.
    /// </summary>
    public SemanticModel SemanticModel => Candidate.SemanticModel;
}
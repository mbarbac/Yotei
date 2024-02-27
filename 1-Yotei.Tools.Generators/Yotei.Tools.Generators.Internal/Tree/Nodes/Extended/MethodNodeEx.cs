namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method node in the source code generation hierarchy built from a given
/// candidate.
/// </summary>
/// <param name="parent"></param>
/// <param name="candidate"></param>
internal class MethodNodeEx(
    TypeNode parent, MethodCandidate candidate) : MethodNode(parent, candidate.Symbol)
{
    /// <summary>
    /// The candidate this instance is associated with.
    /// </summary>
    public MethodCandidate Candidate { get; } = candidate.ThrowWhenNull();

    /// <summary>
    /// The syntax this instance is associated with.
    /// </summary>
    public MethodDeclarationSyntax Syntax => Candidate.Syntax;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a method-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal class MethodCandidate(
    SemanticModel model, MethodDeclarationSyntax node, IMethodSymbol symbol)
    : Candidate(model, node, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.Name}";

    /// <inheritdoc/>
    new public MethodDeclarationSyntax SyntaxNode => (MethodDeclarationSyntax)base.SyntaxNode;

    /// <inheritdoc/>
    new public IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;
}
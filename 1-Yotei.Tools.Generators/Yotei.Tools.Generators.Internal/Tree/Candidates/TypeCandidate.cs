namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a type-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal class TypeCandidate(
    SemanticModel model, TypeDeclarationSyntax node, INamedTypeSymbol symbol)
    : Candidate(model, node, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <inheritdoc/>
    new public TypeDeclarationSyntax SyntaxNode => (TypeDeclarationSyntax)base.SyntaxNode;

    /// <inheritdoc/>
    new public INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;
}
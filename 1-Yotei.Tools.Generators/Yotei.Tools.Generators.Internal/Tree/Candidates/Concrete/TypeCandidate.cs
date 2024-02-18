namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a type-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class TypeCandidate(
    SemanticModel model, TypeDeclarationSyntax syntaxNode, INamedTypeSymbol symbol)
    : ICandidate
{
    /// <inheritdoc cref="ICandidate.SemanticModel"/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public TypeDeclarationSyntax SyntaxNode { get; } = syntaxNode.ThrowWhenNull();
    SyntaxNode ICandidate.SyntaxNode => SyntaxNode;

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public INamedTypeSymbol Symbol { get; } = symbol.ThrowWhenNull();
    ISymbol ICandidate.Symbol => Symbol;
}
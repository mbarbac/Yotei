namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a method-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class MethodCandidate(
    SemanticModel model, MethodDeclarationSyntax syntaxNode, IMethodSymbol symbol)
    : ICandidate
{
    /// <inheritdoc cref="ICandidate.SemanticModel"/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public MethodDeclarationSyntax SyntaxNode { get; } = syntaxNode.ThrowWhenNull();
    SyntaxNode ICandidate.SyntaxNode => SyntaxNode;

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public IMethodSymbol Symbol { get; } = symbol.ThrowWhenNull();
    ISymbol ICandidate.Symbol => Symbol;
}
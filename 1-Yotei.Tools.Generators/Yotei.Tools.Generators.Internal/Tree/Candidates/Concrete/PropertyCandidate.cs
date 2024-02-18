namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a property-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class PropertyCandidate(
    SemanticModel model, PropertyDeclarationSyntax syntaxNode, IPropertySymbol symbol)
    : ICandidate
{
    /// <inheritdoc cref="ICandidate.SemanticModel"/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public PropertyDeclarationSyntax SyntaxNode { get; } = syntaxNode.ThrowWhenNull();
    SyntaxNode ICandidate.SyntaxNode => SyntaxNode;

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public IPropertySymbol Symbol { get; } = symbol.ThrowWhenNull();
    ISymbol ICandidate.Symbol => Symbol;
}
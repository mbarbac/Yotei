namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a field-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class FieldCandidate(
    SemanticModel model, FieldDeclarationSyntax syntaxNode, IFieldSymbol symbol)
    : ICandidate
{
    /// <inheritdoc cref="ICandidate.SemanticModel"/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public FieldDeclarationSyntax SyntaxNode { get; } = syntaxNode.ThrowWhenNull();
    SyntaxNode ICandidate.SyntaxNode => SyntaxNode;

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public IFieldSymbol Symbol { get; } = symbol.ThrowWhenNull();
    ISymbol ICandidate.Symbol => Symbol;
}
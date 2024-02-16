namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a field-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal class FieldCandidate(
    SemanticModel model, FieldDeclarationSyntax node, IFieldSymbol symbol)
    : Candidate(model, node, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Field: {Symbol.Name}";

    /// <inheritdoc/>
    new public FieldDeclarationSyntax SyntaxNode => (FieldDeclarationSyntax)base.SyntaxNode;

    /// <inheritdoc/>
    new public IFieldSymbol Symbol => (IFieldSymbol)base.Symbol;
}
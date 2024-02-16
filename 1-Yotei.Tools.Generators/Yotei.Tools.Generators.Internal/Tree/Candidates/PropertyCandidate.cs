namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a property-alike syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal class PropertyCandidate(
    SemanticModel model, PropertyDeclarationSyntax node, IPropertySymbol symbol)
    : Candidate(model, node, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Property: {Symbol.Name}";

    /// <inheritdoc/>
    new public PropertyDeclarationSyntax SyntaxNode => (PropertyDeclarationSyntax)base.SyntaxNode;

    /// <inheritdoc/>
    new public IPropertySymbol Symbol => (IPropertySymbol)base.Symbol;
}
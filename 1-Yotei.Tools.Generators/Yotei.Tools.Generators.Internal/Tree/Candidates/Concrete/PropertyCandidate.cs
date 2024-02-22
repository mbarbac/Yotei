namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a property-alike syntax node.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class PropertyCandidate(
    SemanticModel model, PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Property: {Symbol.Name}";

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public new PropertyDeclarationSyntax SyntaxNode => (PropertyDeclarationSyntax)base.SyntaxNode;

    /// <inheritdoc cref="ICandidate.Symbol"/>
    public new IPropertySymbol Symbol => (IPropertySymbol)base.Symbol;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a type-alike syntax node.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class TypeCandidate(
    SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <inheritdoc cref="ICandidate.SyntaxNode"/>
    public new TypeDeclarationSyntax SyntaxNode => (TypeDeclarationSyntax)base.SyntaxNode;

    /// <inheritdoc cref="ICandidate.Symbol"/>
    public new INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;
}
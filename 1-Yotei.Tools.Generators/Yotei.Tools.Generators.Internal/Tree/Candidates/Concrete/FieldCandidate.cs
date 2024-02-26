namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a field-alike syntax node.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class FieldCandidate(
    SemanticModel model, FieldDeclarationSyntax syntax, IFieldSymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Field: {Symbol.Name}";

    /// <inheritdoc cref="ICandidate.Syntax"/>
    public new FieldDeclarationSyntax Syntax => (FieldDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="ICandidate.Symbol"/>
    public new IFieldSymbol Symbol => (IFieldSymbol)base.Symbol;
}
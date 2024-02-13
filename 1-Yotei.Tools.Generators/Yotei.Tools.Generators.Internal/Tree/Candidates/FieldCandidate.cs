namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field-alike candidate for source code generation purposes.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class FieldCandidate(
    SemanticModel model, FieldDeclarationSyntax syntax, IFieldSymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Field: {Symbol.EasyName()}";

    /// <inheritdoc cref="Candidate.Syntax"/>
    public new FieldDeclarationSyntax Syntax => (FieldDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="Candidate.Symbol"/>
    public new IFieldSymbol Symbol => (IFieldSymbol)base.Symbol;
}
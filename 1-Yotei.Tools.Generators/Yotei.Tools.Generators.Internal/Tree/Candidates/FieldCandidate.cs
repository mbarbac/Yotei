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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.EasyName()}";

    /// <summary>
    /// <inheritdoc cref="Candidate.Syntax"/>
    /// </summary>
    public new FieldDeclarationSyntax Syntax => (FieldDeclarationSyntax)base.Syntax;

    /// <summary>
    /// <inheritdoc cref="Candidate.Symbol"/>
    /// </summary>
    public new IFieldSymbol Symbol => (IFieldSymbol)base.Symbol;
}
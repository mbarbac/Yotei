namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a property-alike candidate for source code generation purposes.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class PropertyCandidate(
    SemanticModel model, PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Property: {Symbol.EasyName()}";

    /// <inheritdoc cref="Candidate.Syntax"/>
    public new PropertyDeclarationSyntax Syntax => (PropertyDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="Candidate.Symbol"/>
    public new IPropertySymbol Symbol => (IPropertySymbol)base.Symbol;
}
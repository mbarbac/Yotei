namespace Yotei.Tools.Generators.Shared;

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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Property: {Symbol.ToStringEx(useSymbolType: true)}";

    /// <summary>
    /// <inheritdoc cref="Candidate.Syntax"/>
    /// </summary>
    public new PropertyDeclarationSyntax Syntax => (PropertyDeclarationSyntax)base.Syntax;

    /// <summary>
    /// <inheritdoc cref="Candidate.Symbol"/>
    /// </summary>
    public new IPropertySymbol Symbol => (IPropertySymbol)base.Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string GetFileName() => GetTypeFileName();
}
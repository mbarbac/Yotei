namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a property-alike candidate for source code generation purposes.
/// </summary>
internal class PropertyCandidate : Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public PropertyCandidate(
        SemanticModel semanticModel, PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
        : base(semanticModel, syntax, symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Property: {Symbol.ToStringEx(useSymbolType: false)}";

    /// <summary>
    /// The syntax node this instance is associated with.
    /// </summary>
    public new PropertyDeclarationSyntax Syntax => (PropertyDeclarationSyntax)base.Syntax;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public new IPropertySymbol Symbol => (IPropertySymbol)base.Symbol;
}
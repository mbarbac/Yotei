namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike candidate for source code generation purposes.
/// </summary>
internal sealed class PropertyCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public PropertyCandidate(
        ImmutableArray<AttributeData> attributes,
        SemanticModel model,
        PropertyDeclarationSyntax syntax,
        IPropertySymbol symbol)
    {
        Attributes = attributes;
        SemanticModel = model;
        Syntax = syntax;
        Symbol = symbol;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Property: {Symbol.EasyName()}";

    /// <inheritdoc/>
    public ImmutableArray<AttributeData> Attributes { get; }

    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; }

    /// <inheritdoc/>
    public PropertyDeclarationSyntax Syntax { get; }
    SyntaxNode IValidCandidate.Syntax => Syntax;

    /// <inheritdoc/>
    public IPropertySymbol Symbol { get; }
    ISymbol IValidCandidate.Symbol => Symbol;
}
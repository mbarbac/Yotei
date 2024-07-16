namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a property-alike candidate for source code generation.
/// </summary>
internal class PropertyCandidate : Candidate
{
    /// <summary>
    /// Initializes a new valid instance.
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
        : base(attributes, model, syntax, symbol) { }

    /// <inheritdoc/>
    public override string ToString() => $"Property: {Symbol.EasyName()}";

    /// <inheritdoc cref="INodeCandidate.Syntax"/>
    public new PropertyDeclarationSyntax Syntax => (PropertyDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="INodeCandidate.Symbol"/>
    public new IPropertySymbol Symbol => (IPropertySymbol)base.Symbol;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field-alike candidate for source code generation.
/// </summary>
internal class FieldCandidate : Candidate
{
    /// <summary>
    /// Initializes a new valid instance.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public FieldCandidate(
        ImmutableArray<AttributeData> attributes,
        SemanticModel model,
        FieldDeclarationSyntax syntax,
        IFieldSymbol symbol)
        : base(attributes, model, syntax, symbol) { }

    /// <inheritdoc/>
    public override string ToString() => $"Field: {Symbol.EasyName()}";

    /// <inheritdoc cref="INodeCandidate.Syntax"/>
    public new FieldDeclarationSyntax Syntax => (FieldDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="INodeCandidate.Symbol"/>
    public new IFieldSymbol Symbol => (IFieldSymbol)base.Symbol;
}
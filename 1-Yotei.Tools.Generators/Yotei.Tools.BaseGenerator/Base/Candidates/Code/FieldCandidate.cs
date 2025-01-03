namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike candidate for source code generation purposes.
/// </summary>
internal sealed class FieldCandidate : ICandidate
{
    /// <summary>
    /// Initializes a new instance.
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
    {
        Attributes = attributes;
        SemanticModel = model;
        Syntax = syntax;
        Symbol = symbol;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Field: {Symbol.EasyName()}";

    /// <inheritdoc/>
    public ImmutableArray<AttributeData> Attributes { get; }

    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; }

    /// <inheritdoc/>
    public FieldDeclarationSyntax Syntax { get; }
    SyntaxNode ICandidate.Syntax => Syntax;

    /// <inheritdoc/>
    public IFieldSymbol Symbol { get; }
    ISymbol ICandidate.Symbol => Symbol;
}
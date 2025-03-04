namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike candidate for source code generation purposes.
/// </summary>
internal sealed class TypeCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public TypeCandidate(
        ImmutableArray<AttributeData> attributes,
        SemanticModel model,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol)
    {
        Attributes = attributes;
        SemanticModel = model;
        Syntax = syntax;
        Symbol = symbol;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.EasyName()}";

    /// <inheritdoc/>
    public ImmutableArray<AttributeData> Attributes { get; }

    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; }

    /// <inheritdoc/>
    public TypeDeclarationSyntax Syntax { get; }
    SyntaxNode IValidCandidate.Syntax => Syntax;

    /// <inheritdoc/>
    public INamedTypeSymbol Symbol { get; }
    ISymbol IValidCandidate.Symbol => Symbol;
}
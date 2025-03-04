namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike candidate for source code generation purposes.
/// </summary>
internal sealed class MethodCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public MethodCandidate(
        ImmutableArray<AttributeData> attributes,
        SemanticModel model,
        MethodDeclarationSyntax syntax,
        IMethodSymbol symbol)
    {
        Attributes = attributes;
        SemanticModel = model;
        Syntax = syntax;
        Symbol = symbol;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.EasyName()}";

    /// <inheritdoc/>
    public ImmutableArray<AttributeData> Attributes { get; }

    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; }

    /// <inheritdoc/>
    public MethodDeclarationSyntax Syntax { get; }
    SyntaxNode IValidCandidate.Syntax => Syntax;

    /// <inheritdoc/>
    public IMethodSymbol Symbol { get; }
    ISymbol IValidCandidate.Symbol => Symbol;
}
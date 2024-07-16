namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method-alike candidate for source code generation.
/// </summary>
internal class MethodCandidate : Candidate
{
    /// <summary>
    /// Initializes a new valid instance.
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
        : base(attributes, model, syntax, symbol) { }

    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.EasyName()}";

    /// <inheritdoc cref="INodeCandidate.Syntax"/>
    public new MethodDeclarationSyntax Syntax => (MethodDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="INodeCandidate.Symbol"/>
    public new IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method-alike candidate for source code generation purposes.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class MethodCandidate(
    SemanticModel model, MethodDeclarationSyntax syntax, IMethodSymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.EasyName()}";

    /// <inheritdoc cref="Candidate.Syntax"/>
    public new MethodDeclarationSyntax Syntax => (MethodDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="Candidate.Symbol"/>
    public new IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a method-alike syntax node.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class MethodCandidate(
    SemanticModel model, MethodDeclarationSyntax syntax, IMethodSymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.Name}";

    /// <inheritdoc cref="ICandidate.Syntax"/>
    public new MethodDeclarationSyntax Syntax => (MethodDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="ICandidate.Symbol"/>
    public new IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;
}
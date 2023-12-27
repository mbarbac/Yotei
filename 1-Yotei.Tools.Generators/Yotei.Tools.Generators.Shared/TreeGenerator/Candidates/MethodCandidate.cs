namespace Yotei.Tools.Generators.Shared;

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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Method: {Symbol.ToStringEx(useReturnType: true)}";

    /// <summary>
    /// <inheritdoc cref="Candidate.Syntax"/>
    /// </summary>
    public new MethodDeclarationSyntax Syntax => (MethodDeclarationSyntax)base.Syntax;

    /// <summary>
    /// <inheritdoc cref="Candidate.Symbol"/>
    /// </summary>
    public new IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string GetFileName() => GetTypeFileName();
}
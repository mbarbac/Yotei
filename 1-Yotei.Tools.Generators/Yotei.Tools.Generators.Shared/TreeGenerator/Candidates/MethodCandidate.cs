namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a method-alike candidate for source code generation purposes.
/// </summary>
internal class MethodCandidate : Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public MethodCandidate(
        SemanticModel semanticModel, MethodDeclarationSyntax syntax, IMethodSymbol symbol)
        : base(semanticModel, syntax, symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Method: {Symbol.ToStringEx(useReturnType: false)}";

    /// <summary>
    /// The syntax node this instance is associated with.
    /// </summary>
    public new MethodDeclarationSyntax Syntax => (MethodDeclarationSyntax)base.Syntax;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public new IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;
}
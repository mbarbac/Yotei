namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a method-alike candidate for source code generation purposes.
/// </summary>
internal class MethodCandidate : Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public MethodCandidate(
        SemanticModel model, MethodDeclarationSyntax syntax, IMethodSymbol symbol)
        : base(model, syntax, symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Method: {Symbol.ToStringEx(useReturnType: true)}";

    /// <summary>
    /// The syntax node this instance is associated with.
    /// </summary>
    public new MethodDeclarationSyntax Syntax => (MethodDeclarationSyntax)base.Syntax;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public new IMethodSymbol Symbol => (IMethodSymbol)base.Symbol;
}
namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a type-alike candidate for source code generation purposes.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class TypeCandidate(
    SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    : Candidate(model, syntax, symbol)
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// <inheritdoc cref="Candidate.Symbol"/>
    /// </summary>
    public new TypeDeclarationSyntax Syntax => (TypeDeclarationSyntax)base.Syntax;

    /// <summary>
    /// <inheritdoc cref="Candidate.Symbol"/>
    /// </summary>
    public new INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string GetFileName() => GetTypeFileName();
}
namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a type-alike candidate for source code generation purposes.
/// </summary>
internal class TypeCandidate : Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public TypeCandidate(
        SemanticModel semanticModel, TypeDeclarationSyntax syntax, ITypeSymbol symbol)
        : base(semanticModel, syntax, symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// The syntax node this instance is associated with.
    /// </summary>
    public new TypeDeclarationSyntax Syntax => (TypeDeclarationSyntax)base.Syntax;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public new ITypeSymbol Symbol => (ITypeSymbol)base.Symbol;
}
namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a type-alike candidate for source code generation purposes.
/// </summary>
internal class TypeCandidate : Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public TypeCandidate(
        SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        : base(model, syntax, symbol) { }

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
    public new INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;
}
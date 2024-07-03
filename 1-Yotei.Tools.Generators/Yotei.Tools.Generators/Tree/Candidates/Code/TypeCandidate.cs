namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type-alike candidate for source code generation.
/// </summary>
internal class TypeCandidate : Candidate
{
    /// <summary>
    /// Initializes a new valid instance.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public TypeCandidate(
        ImmutableArray<AttributeData> attributes,
        SemanticModel model,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol)
        : base(attributes, model, syntax, symbol) { }

    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <inheritdoc cref="INodeCandidate.Syntax"/>
    public new TypeDeclarationSyntax Syntax => (TypeDeclarationSyntax)base.Syntax;

    /// <inheritdoc cref="INodeCandidate.Symbol"/>
    public new INamedTypeSymbol Symbol => (INamedTypeSymbol)base.Symbol;
}
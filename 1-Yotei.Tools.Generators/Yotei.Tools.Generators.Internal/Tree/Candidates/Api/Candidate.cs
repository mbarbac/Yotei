namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="ICandidate"/>
internal abstract class Candidate(SemanticModel model, SyntaxNode syntax, ISymbol symbol)
    : ICandidate
{
    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc/>
    public SyntaxNode Syntax { get; } = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public ISymbol Symbol { get; } = symbol.ThrowWhenNull();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain => _NamespaceSyntaxChain ??= Syntax.GetNamespaceSyntaxChain();
    public ImmutableArray<BaseNamespaceDeclarationSyntax>? _NamespaceSyntaxChain;

    /// <inheritdoc/>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain => _TypeSyntaxChain ??= Syntax.GetTypeSyntaxChain();
    public ImmutableArray<TypeDeclarationSyntax>? _TypeSyntaxChain;

    /// <inheritdoc/>
    public ImmutableArray<INamedTypeSymbol> TypeSymbolChain => _TypeSymbolChain ??= Symbol.GetTypeSymbolChain();
    public ImmutableArray<INamedTypeSymbol>? _TypeSymbolChain;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a method-alike syntax node.
/// </summary>
internal class MethodCandidate(
    SemanticModel model, MethodDeclarationSyntax syntax, IMethodSymbol symbol) : ICandidate
{
    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.Name}";

    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc cref="ICandidate.Syntax"/>
    public MethodDeclarationSyntax Syntax { get; } = syntax.ThrowWhenNull();
    SyntaxNode ICandidate.Syntax => Syntax;

    /// <inheritdoc cref="ICandidate.Symbol"/>
    public IMethodSymbol Symbol { get; } = symbol.ThrowWhenNull();
    ISymbol ICandidate.Symbol => symbol;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain => _NamespaceSyntaxChain ??= Syntax.GetNamespaceSyntaxChain();
    ImmutableArray<BaseNamespaceDeclarationSyntax>? _NamespaceSyntaxChain;

    /// <inheritdoc/>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain => _TypeSyntaxChain ??= Syntax.GetTypeSyntaxChain();
    ImmutableArray<TypeDeclarationSyntax>? _TypeSyntaxChain;

    /// <inheritdoc/>
    public ImmutableArray<INamedTypeSymbol> TypeSymbolChain => _TypeSymbolChain ??= Symbol.GetTypeSymbolChain();
    ImmutableArray<INamedTypeSymbol>? _TypeSymbolChain;
}
namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICapturedType"/>
/// </summary>
internal class CapturedType : ICapturedType
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public CapturedType(
        IGenerator generator,
        TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel model)
    {
        Generator = generator.ThrowIfNull(nameof(generator));
        Syntax = syntax.ThrowIfNull(nameof(syntax));
        Symbol = symbol.ThrowIfNull(nameof(symbol));
        SemanticModel = model.ThrowIfNull(nameof(model));

        Name = symbol.ShortName();
        NamespaceSyntaxChain = syntax.NamespaceSyntaxChain();
        TypeSyntaxChain = syntax.TypeSyntaxChain();
        TypeSymbolChain = symbol.TypeSymbolChain();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Type: {Name}";

    /// <summary>
    /// The generator this node refers to.
    /// </summary>
    public IGenerator Generator { get; }

    /// <summary>
    /// The name of this instance.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TypeDeclarationSyntax Syntax { get; }
    SyntaxNode ICaptured.Syntax => Syntax;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; }
    ISymbol ICaptured.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SemanticModel SemanticModel { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ImmutableArray<INamedTypeSymbol> TypeSymbolChain { get; }
}
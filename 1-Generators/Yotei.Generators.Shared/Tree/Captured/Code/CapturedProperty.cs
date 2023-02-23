namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICapturedProperty"/>
/// </summary>
internal class CapturedProperty : ICapturedProperty
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public CapturedProperty(
        IGenerator generator,
        ICapturedType parent,
        PropertyDeclarationSyntax syntax, IPropertySymbol symbol, SemanticModel model)
    {
        Generator = generator.ThrowIfNull(nameof(generator));
        Parent = parent.ThrowIfNull(nameof(parent));
        Syntax = syntax.ThrowIfNull(nameof(syntax));
        Symbol = symbol.ThrowIfNull(nameof(symbol));
        SemanticModel = model.ThrowIfNull(nameof(model));
        Name = symbol.ShortName();
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
    public ICapturedType Parent { get; }

    /// <summary>
    /// <inheritdoc cref="ICaptured.Syntax"/>
    /// </summary>
    public PropertyDeclarationSyntax Syntax { get; }
    SyntaxNode ICaptured.Syntax => Syntax;

    /// <summary>
    /// <inheritdoc cref="ICaptured.Symbol"/>
    /// </summary>
    public IPropertySymbol Symbol { get; }
    ISymbol ICaptured.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SemanticModel SemanticModel { get; }
}
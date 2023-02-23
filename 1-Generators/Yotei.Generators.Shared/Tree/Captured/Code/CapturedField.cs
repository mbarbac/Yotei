namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICapturedField"/>
/// </summary>
internal class CapturedField : ICapturedField
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public CapturedField(
        IGenerator generator,
        ICapturedType parent,
        FieldDeclarationSyntax syntax, IFieldSymbol symbol, SemanticModel model)
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
    public FieldDeclarationSyntax Syntax { get; }
    SyntaxNode ICaptured.Syntax => Syntax;

    /// <summary>
    /// <inheritdoc cref="ICaptured.Symbol"/>
    /// </summary>
    public IFieldSymbol Symbol { get; }
    ISymbol ICaptured.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SemanticModel SemanticModel { get; }
}
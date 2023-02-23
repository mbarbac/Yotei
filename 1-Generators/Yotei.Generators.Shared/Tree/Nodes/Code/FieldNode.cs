namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="IFieldNode"/>
/// </summary>
internal class FieldNode : IFieldNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public FieldNode(
        ITypeNode parent,
        FieldDeclarationSyntax syntax, IFieldSymbol symbol, SemanticModel model)
    {
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
    public override string ToString() => $"Field: {Name}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IGenerator Generator => Parent.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITypeNode Parent { get; }
    INode INode.Parent => Parent;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public FieldDeclarationSyntax Syntax { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IFieldSymbol Symbol { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SemanticModel SemanticModel { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public virtual bool Validate(SourceProductionContext context) => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Print(SourceProductionContext context, CodeBuilder cb) { }
}
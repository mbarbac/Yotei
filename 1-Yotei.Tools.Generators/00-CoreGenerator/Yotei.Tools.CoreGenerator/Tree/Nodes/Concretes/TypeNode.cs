namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike source code generation element.
/// </summary>
internal class TypeNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public TypeNode(INamedTypeSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// By default, the collection of syntax nodes where the associated symbol was found, or an
    /// empty one. Its members are instances of the following types:
    /// <see cref="EnumDeclarationSyntax"/> and <see cref="TypeDeclarationSyntax"/>.
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// By default, the collection of attributes by which the associated symbol was found, or an
    /// empty one.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child properties known to this instance.
    /// </summary>
    public List<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields known to this instance.
    /// </summary>
    public List<FieldNode> ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods known to this instance.
    /// </summary>
    public List<MethodNode> ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Augments the contents of this instance with the ones obtained from the given one. The
    /// default implementation just adds the syntax nodes and attributes to their respective
    /// collections.
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(TypeNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);

        var comparer = SymbolEqualityComparer.Default;

        foreach (var item in node.ChildProperties)
            if (ChildProperties.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildProperties.Add(item);

        foreach (var item in node.ChildFields)
            if (ChildFields.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildFields.Add(item);

        foreach (var item in node.ChildMethods)
            if (ChildMethods.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildMethods.Add(item);
    }
}
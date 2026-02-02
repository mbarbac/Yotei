namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike node in the source code generation hierarchy.
/// </summary>
internal class PropertyNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public PropertyNode(TypeNode parent, IPropertySymbol symbol)
    {
        Parent = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Property: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// The node this instance belong to in the source code generation hierarchy. Note that it
    /// may not represent the containing element.
    /// </summary>
    public TypeNode Parent { get; }

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public IPropertySymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured for this instance, or an empty one if any.
    /// </summary>
    public List<BasePropertyDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The collection of attributes by which this candidate was identified
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// candidate. This method is invoked by the hierarchy-creation process when a node for the
    /// element already exist in that hierarchy.
    /// </summary>
    /// <param name="candidate"></param>
    public virtual void Augment(PropertyCandidate candidate)
    {
        if (candidate.Syntax is not null)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(candidate.Syntax)) == null)
                SyntaxNodes.Add(candidate.Syntax);

        foreach (var at in candidate.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// node. This method is invoked by the hierarchy-creation process when a node for the element
    /// already exist in that hierarchy.
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(PropertyNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(
        SourceProductionContext context, CodeBuilder cb) => cb.AppendLine($"// {this}");
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike node in the source code generation hierarchy.
/// </summary>
internal class FieldNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public FieldNode(TypeNode parent, IFieldSymbol symbol)
    {
        Parent = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.Name}";

    /// <summary>
    /// <inheritdoc cref="IChildNode.Parent"/>
    /// </summary>
    public TypeNode Parent { get; }
    INode IChildNode.Parent => Parent;

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public IFieldSymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured for instance. This Field can be an empty one
    /// if this information was not captured.
    /// </summary>
    public List<FieldDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The collection of attributes by which this instance was found. This Field can be an empty
    /// collection if this instance was created just to hold its child elements.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IChildNode.Augment(ICandidate)"/>
    /// </summary>
    /// <param name="candidate"></param>
    public virtual void Augment(FieldCandidate candidate)
    {
        if (candidate.Syntax is not null)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(candidate.Syntax)) == null)
                SyntaxNodes.Add(candidate.Syntax);

        foreach (var at in candidate.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }
    void IChildNode.Augment(ICandidate candidate) => Augment((FieldCandidate)candidate);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IChildNode.Augment(INode)"/>
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(FieldNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }
    void IChildNode.Augment(INode node) => Augment((FieldNode)node);

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
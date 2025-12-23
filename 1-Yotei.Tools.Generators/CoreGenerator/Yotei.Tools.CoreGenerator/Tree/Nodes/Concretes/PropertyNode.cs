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
        Symbol = symbol.ThrowWhenNull();
        Parent = parent.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// The node this instance belongs to in the source code generation hierarchy. Note that the
    /// parent node's type might not be the same as this instance's declaring one.
    /// </summary>
    public TypeNode Parent { get; }

    /// <summary>
    /// <inheritdoc cref="INode.Symbol"/>
    /// </summary>
    public IPropertySymbol Symbol { get; }
    ISymbol INode.Symbol => Symbol;

    /// <summary>
    /// The collection of syntax nodes where the element represented by this instance was found.
    /// </summary>
    public List<PropertyDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The collection of attributes captured for the element represented by this instance, at the
    /// syntax nodes where it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="INode.Augment(ICandidate)"/>
    /// </summary>
    /// <param name="candidate"></param>
    public virtual void Augment(PropertyCandidate candidate)
    {
        if (candidate.Syntax != null) SyntaxNodes.Add(candidate.Syntax);
        Attributes.AddRange(candidate.Attributes);
    }
    void INode.Augment(ICandidate candidate) => Augment((PropertyCandidate)candidate);

    /// <summary>
    /// <inheritdoc cref="INode.Augment(INode)"/>
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(PropertyNode node)
    {
        SyntaxNodes.AddRange(node.SyntaxNodes);
        Attributes.AddRange(node.Attributes);
    }
    void INode.Augment(INode node) => Augment((PropertyNode)node);

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
        SourceProductionContext context, CodeBuilder cb) => cb.Append($"// {this}");
}
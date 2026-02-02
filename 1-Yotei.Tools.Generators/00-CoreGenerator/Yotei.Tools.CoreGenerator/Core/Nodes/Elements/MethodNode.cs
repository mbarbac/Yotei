namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public MethodNode(TypeNode parent, IMethodSymbol symbol)
    {
        Parent = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"Method: {Symbol.Name}");
        sb.Append('(');
        for (int i = 0; i < Symbol.Parameters.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(Symbol.Parameters[i].Type.Name);
        }
        sb.Append(')');
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The node this instance belong to in the source code generation hierarchy. Note that it
    /// may not represent the containing element.
    /// </summary>
    public TypeNode Parent { get; }

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public IMethodSymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured for this instance, or an empty one if any.
    /// </summary>
    public List<BaseMethodDeclarationSyntax> SyntaxNodes { get; } = [];

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
    public virtual void Augment(MethodCandidate candidate)
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
    public virtual void Augment(MethodNode node)
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
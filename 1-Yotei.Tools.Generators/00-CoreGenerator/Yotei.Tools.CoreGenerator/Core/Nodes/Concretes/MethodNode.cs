namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : IChildNode
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

    /// <summary>
    /// <inheritdoc cref="IChildNode.Parent"/>
    /// </summary>
    public TypeNode Parent { get; }
    INode IChildNode.Parent => Parent;

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public IMethodSymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured for instance. This Method can be an empty one
    /// if this information was not captured.
    /// </summary>
    public List<BaseMethodDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The collection of attributes by which this instance was found. This Method can be an empty
    /// collection if this instance was created just to hold its child elements.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IChildNode.Augment(ICandidate)"/>
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
    void IChildNode.Augment(ICandidate candidate) => Augment((MethodCandidate)candidate);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IChildNode.Augment(INode)"/>
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
    void IChildNode.Augment(INode node) => Augment((MethodNode)node);

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
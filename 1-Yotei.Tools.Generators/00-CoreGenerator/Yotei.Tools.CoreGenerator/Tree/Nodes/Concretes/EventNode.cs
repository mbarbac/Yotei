namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a event-alike source code generation element.
/// </summary>
internal class EventNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public EventNode(IEventSymbol symbol) => Symbol = symbol.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Event: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.ParentNode"/>
    /// </summary>
    public TypeNode? ParentNode { get; set; }
    INode? ITreeNode.ParentNode => ParentNode;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public IEventSymbol Symbol { get; }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.SyntaxNodes"/> Elements shall either be instances of
    /// <see cref="EventDeclarationSyntax"/> or <see cref="EventFieldDeclarationSyntax"/>.
    /// </summary>
    public List<MemberDeclarationSyntax> SyntaxNodes { get; } = [];
    List<SyntaxNode> ITreeNode.SyntaxNodes
        => (List<SyntaxNode>)SyntaxNodes.Cast<MemberDeclarationSyntax>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Augment(EventNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }
    void ITreeNode.Augment(ITreeNode node) => Augment((EventNode)node);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this node at source code generation time. If this method returns
    /// <see langword="false"/>, source code generation is aborted. Derived types must invoke
    /// their base ones first.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        var r = true;
        if (ParentNode == null) { Symbol.ReportError(TreeError.NoParentNode, context); r = false; }
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var r = OnValidate(context) && OnEmit(context, cb);
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to generate this node's source code, once it has been validated.
    /// <br/> Derived types must override this method.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmit(SourceProductionContext context, CodeBuilder cb) => true;
}
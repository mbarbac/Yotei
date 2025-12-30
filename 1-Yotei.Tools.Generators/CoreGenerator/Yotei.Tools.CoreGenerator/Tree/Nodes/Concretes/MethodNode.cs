namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : ITreeNode
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
    public override string ToString() => $"Method: {Symbol.EasyName()}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Parent"/>
    /// </summary>
    public TypeNode Parent { get; }
    INode ITreeNode.Parent => Parent;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public IMethodSymbol Symbol { get; }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// The syntax nodes captured for this instance, or an empty collection if this information
    /// was not captured.
    /// </summary>
    public List<BaseMethodDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The attributes by which this instance was found, or an empty collection if this information
    /// was not captured.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc <see cref="ITreeNode.Augment(IValidCandidate)"/>/>
    /// </summary>
    /// <param name="candidate"></param>
    public virtual void Augment(MethodCandidate candidate)
    {
        if (candidate.Syntax != null)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(candidate.Syntax)) == null)
                SyntaxNodes.Add(candidate.Syntax);

        foreach (var at in candidate.Attributes)
            if (Attributes.Find(x => x.EqualTo(at)) == null)
                Attributes.Add(at);
    }
    void ITreeNode.Augment(IValidCandidate candidate) => Augment((MethodCandidate)candidate);

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Augment(ITreeNode)"/>
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(MethodNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualTo(at)) == null)
                Attributes.Add(at);
    }
    void ITreeNode.Augment(ITreeNode node) => Augment((MethodNode)node);

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
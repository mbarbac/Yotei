namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a property-alike source code generation node.
/// </summary>
public class PropertyNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public PropertyNode(IPropertySymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Property: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IChildNode.Parent"/>
    /// </summary>
    public TypeNode? Parent { get; set; }
    ITreeNode? IChildNode.Parent { get => Parent; set => Parent = (TypeNode?)value; }

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public IPropertySymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.SyntaxNodes"/>
    /// </summary>
    public List<BasePropertyDeclarationSyntax> SyntaxNodes { get; } = [];
    List<SyntaxNode> ITreeNode.SyntaxNodes => (List<SyntaxNode>)SyntaxNodes.Cast<SyntaxNode>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not PropertyNode valid) return false;

        return SymbolEqualityComparer.Default.Equals(Symbol, valid.Symbol);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(Symbol);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Augment(ITreeNode)"/>
    /// </summary>
    /// <param name="other"></param>
    public virtual void Augment(PropertyNode other)
    {
        foreach (var syntax in other.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in other.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }
    void ITreeNode.Augment(ITreeNode other) => Augment((PropertyNode)other);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> Inheritors will customize behaviors by overriding the
    /// <see cref="OnValidate(SourceProductionContext)"/> and the
    /// <see cref="OnEmit(ref TreeContext)"/> methods.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    public bool Emit(ref TreeContext context, CodeBuilder cb)
    {
        context.Context.CancellationToken.ThrowIfCancellationRequested();

        if (!OnValidate(context.Context)) return false;
        if (!OnEmit(ref context, cb)) return false;
        return true;
    }

    /// <summary>
    /// Invoked to validate this instance before emitting its source code. If this method returns
    /// false, then the source code generation is aborted. Inheritors typically will invoke their
    /// base methods first.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        var r = true;

        if (Parent == null) { TreeError.NoParentNode.Report(Symbol, context); r = false; }
        return r;
    }

    /// <summary>
    /// Invoked to generate the source code associated with this instance. If this method returns
    /// <see langword="false"/>, then its source code generation will be aborted. Inheritors will
    /// typically override completely this method.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmit(ref TreeContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
        return true;
    }
}
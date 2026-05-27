namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a method-alike source code generation node.
/// </summary>
public partial class MethodNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public MethodNode(IMethodSymbol symbol) => Symbol = symbol;

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
    /// <inheritdoc cref="IChildNode.Parent"/>
    /// </summary>
    public TypeNode? Parent { get; set; }
    ITreeNode? IChildNode.Parent { get => Parent; set => Parent = (TypeNode?)value; }

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public IMethodSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.SyntaxNodes"/>
    /// </summary>
    public List<BaseMethodDeclarationSyntax> SyntaxNodes { get; } = [];
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
    public bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not MethodNode valid) return false;

        if (!SymbolEqualityComparer.Default.Equals(Symbol, valid.Symbol)) return false;
        if (!Attributes.SequenceEqual(valid.Attributes)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int code = SymbolEqualityComparer.Default.GetHashCode(Symbol);
        foreach (var attr in Attributes) code = HashCode.Combine(code, attr);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.With(SyntaxNode, IEnumerable{AttributeData}, SemanticModel)"/>
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual MethodNode With(
        BaseMethodDeclarationSyntax syntax, IEnumerable<AttributeData> attributes, SemanticModel model)
    {
        SyntaxNodes.Add(syntax);
        Attributes.AddRange(attributes);
        return this;
    }

    ITreeNode ITreeNode.With(
        SyntaxNode syntax, IEnumerable<AttributeData> attributes, SemanticModel model)
        => With((BaseMethodDeclarationSyntax)syntax, attributes, model);

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Augment(ITreeNode)"/>
    /// </summary>
    /// <param name="other"></param>
    public virtual void Augment(MethodNode other)
    {
        foreach (var syntax in other.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in other.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }

    void ITreeNode.Augment(ITreeNode other) => Augment((MethodNode)other);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> This method is INFRASTRUCTURE, not intended for inheritors usage. Behavior can be
    /// customized using the <see cref="OnValidate(SourceProductionContext)"/> and the
    /// <see cref="OnEmit(in TreeContext, CodeBuilder)"/> methods.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    public bool Emit(in TreeContext context, CodeBuilder cb)
    {
        context.Context.CancellationToken.ThrowIfCancellationRequested();

        if (!OnValidate(context.Context)) return false;
        if (!OnEmit(in context, cb)) return false;
        return true;
    }

    /// <summary>
    /// Invoked to validate this instance.
    /// <br/> Inheritors may want to invoke their base method first.
    /// <br/> If this method returns false then the code generation of this instance is aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        var r = true;

        if (Parent == null) { TreeError.NoParentNode.Report(Symbol, context); r = false; }

        if (!Symbol.ContainingType.IsPartial)
        { TreeError.TypeNotPartial.Report(Symbol.ContainingType, context); r = false; }
        
        return r;
    }

    /// <summary>
    /// Invoked to generate the source code of this instance.
    /// <br/> If this method returns false then the code generation of this instance is aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmit(in TreeContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
        return true;
    }
}
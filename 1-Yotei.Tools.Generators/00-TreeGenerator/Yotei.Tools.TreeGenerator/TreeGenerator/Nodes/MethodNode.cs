using System.ComponentModel;

namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured method-alike source code generation node.
/// </summary>
public class MethodNode : ITreeNode
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
    /// <inheritdoc cref="ITreeNode.Parent"/>
    /// Instances of this type only accept <see cref="TypeNode"/> parents.
    /// </summary>
    public TypeNode? Parent { get; set; }
    INode? ITreeNode.Parent => Parent;

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
    /// Equality semantics customized for generator caching purposes.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not MethodNode valid) return false;

        return SymbolEqualityComparer.Default.Equals(Symbol, valid.Symbol);
    }

    /// <summary>
    /// <inheritdoc/>
    /// Equality semantics customized for generator caching purposes.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(Symbol);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Augment(ITreeNode)"/>
    /// </summary>
    /// <param name="node"></param>
    public virtual void Augment(MethodNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }
    void ITreeNode.Augment(ITreeNode node) => Augment((MethodNode)node);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool Emit(SourceProductionContext context, CodeBuilder cb)
    {
        if (!OnValidate(context)) return false;
        if (!OnEmit(context, cb)) return false;
        return true;
    }

    /// <summary>
    /// Invoked to validate this node at source code generation time. If this method returns
    /// <see langword="false"/>, then source code generation is aborted.
    /// <br/> Derived types MUST invoke their base methods first.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        var r = true;
        if (Parent is null) { TreeError.NoParentNode.Create(Symbol).Report(context); r = false; }
        return r;
    }

    /// <summary>
    /// Invoked to emit the actual source code generated for this element. If this method returns
    /// <see langword="false"/>, then source code generation is aborted.
    /// <br/> Derived types MUST override this base method.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
        return true;
    }
}
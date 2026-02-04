namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike source code generation element.
/// </summary>
internal class MethodNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
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
    /// The node representing the parent type of this instance in the source code generation tree,
    /// or <see langword="null"/> if not yet captured into it. Its symbol might not neccesarily be
    /// the containing type of this one.
    /// </summary>
    public TypeNode? ParentNode { get; set; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IMethodSymbol Symbol { get; }

    /// <summary>
    /// By default, the collection of syntax nodes where the associated symbol was found, or an
    /// empty one. Its members are by default instances of any the following types:
    /// <see cref="ConstructorDeclarationSyntax"/>, <see cref="ConversionOperatorDeclarationSyntax"/>,
    /// <see cref="DestructorDeclarationSyntax"/>, <see cref="MethodDeclarationSyntax"/>, and
    /// <see cref="OperatorDeclarationSyntax"/>.
    /// </summary>
    public List<BaseMethodDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// By default, the collection of attributes by which the associated symbol was found, or an
    /// empty one.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Augments the contents of this instance with the ones obtained from the given one. The
    /// default implementation just adds the syntax nodes and attributes to their respective
    /// collections.
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
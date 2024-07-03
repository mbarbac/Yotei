namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal sealed class NamespaceNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    public NamespaceNode(INode parent, BaseNamespaceDeclarationSyntax syntax)
    {
        ParentNode = parent.ThrowWhenNull();
        Syntax = syntax.ThrowWhenNull();

        if (ParentNode is not FileNode and not NamespaceNode) throw new ArgumentException(
            "Parent node is not a file nor a namespace.")
            .WithData(parent);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// The parent node this instance belongs to in the source code generation hierarchy, which
    /// can be either a top-most file or a parent namespace.
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The full name of this namespace, including its dot-separated parts, if any.
    /// </summary>
    public string Name => Syntax.Name.ToString();

    /// <summary>
    /// The syntax associated to this namespace.
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; }

    // -----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces registered into this node.
    /// </summary>
    public ChildNamespaces ChildNamespaces { get; } = [];

    /// <summary>
    /// The collection of child types registered into this node.
    /// </summary>
    public ChildTypes ChildTypes { get; } = [];

    // -----------------------------------------------------

    /// <summary>
    /// Invoked before generation to validate this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context) => throw null;

    // -----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace in the source code generation hierarchy.
/// </summary>
internal sealed class NamespaceNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="syntax"></param>
    [SuppressMessage("", "IDE0290")]
    public NamespaceNode(BaseNamespaceDeclarationSyntax syntax) => Syntax = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// The syntax of this namespace.
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The name of this namespace, including its dot separated parts if any.
    /// </summary>
    public string Name => Syntax.Name.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// The node this instance belongs to in the source code generation hiearchy, or null if it
    /// is an orphan one.
    /// </summary>
    public INode? ParentNode
    {
        get => _ParentNode;
        set
        {
            if (ReferenceEquals(_ParentNode, value)) return;

            if (_ParentNode is NamespaceNode nsparent) nsparent.ChildNamespaces.Remove(this);

            if ((_ParentNode = value) != null)
            {
            }
        }
    }
    INode? _ParentNode;

    /// <summary>
    /// The list of namespaces registered in this instance.
    /// </summary>
    public List<NamespaceNode> ChildNamespaces { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}
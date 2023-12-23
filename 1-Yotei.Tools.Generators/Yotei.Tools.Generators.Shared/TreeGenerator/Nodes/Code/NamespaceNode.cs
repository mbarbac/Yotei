namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in a source code generation hierarchy.
/// </summary>
internal class NamespaceNode : INode
{
    /// <summary>
    /// Private constructor.
    /// </summary>
    NamespaceNode(INode parent, string name, bool _)
    {
        ParentNode = parent.ThrowWhenNull();
        Name = name.NotNullNotEmpty();

        NamespaceNodes = new NamespaceList(this);
        TypeNodes = new TypeList(this);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public NamespaceNode(FileBuilder parent, string name) : this(parent, name, false) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public NamespaceNode(NamespaceNode parent, string name) : this(parent, name, false) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public NamespaceNode(FileBuilder parent, BaseNamespaceDeclarationSyntax syntax)
        : this(parent, syntax.Name.ShortName())
        => Syntax = syntax.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public NamespaceNode(NamespaceNode parent, BaseNamespaceDeclarationSyntax syntax)
        : this(parent, syntax.Name.ShortName())
        => Syntax = syntax.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseGenerator Generator => ParentNode.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode ParentNode { get; }
    INode? INode.ParentNode => ParentNode;

    /// <summary>
    /// The file node this instance belong to, or null if this is not a top-most namespace.
    /// </summary>
    public FileBuilder? ParentFileNode => ParentNode as FileBuilder;

    /// <summary>
    /// The type node this instance belong to, or null if this is a top-most namespace.
    /// </summary>
    public NamespaceNode? ParentNamespaceNode => ParentNode as NamespaceNode;

    // ----------------------------------------------------

    /// <summary>
    /// The name of this namespace, including dots if it is a multipart one.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The syntax this instance is associated with, or null if this node has not been created
    /// by the generator.
    /// </summary>
    public BaseNamespaceDeclarationSyntax? Syntax { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of namespace nodes registered in this instance.
    /// </summary>
    public NamespaceList NamespaceNodes { get; }
    public class NamespaceList : CustomList<NamespaceNode>
    {
        readonly NamespaceNode Master;
        public NamespaceList(NamespaceNode master)
        {
            Master = master;
            OnValidate = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                    "Parent node of the given item is not this instance.");
                return item;
            };
            OnCompare = (item, other) => item.Name == other.Name;
            OnSameElement = ReferenceEquals;
            OnAcceptDuplicate = (item, other) => false;
        }
    }

    /// <summary>
    /// The collection of type nodes registered in this instance.
    /// </summary>
    public TypeList TypeNodes { get; }
    public class TypeList : CustomList<TypeNode>
    {
        readonly NamespaceNode Master;
        public TypeList(NamespaceNode master)
        {
            Master = master;
            OnValidate = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                    "Parent node of the given item is not this instance.");
                return item;
            };
            OnCompare = (item, other) => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
            OnSameElement = ReferenceEquals;
            OnAcceptDuplicate = (item, other) => false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Registers the given candidate.
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="nsIndex"></param>
    /// <param name="tpIndex"></param>
    public void Register(Candidate candidate, int nsIndex, int tpIndex)
    {
        candidate.ThrowWhenNull();

        if (nsIndex < candidate.NamespaceSyntaxChain.Length)
        {
            var syntax = candidate.NamespaceSyntaxChain[nsIndex];
            var name = syntax.Name.ShortName();
            var index = NamespaceNodes.IndexOf(x => x.Name == name);

            var node = index >= 0
                ? NamespaceNodes[index]
                : Generator.CreateNode(this, syntax);

            node.Register(candidate, nsIndex + 1, 0);
            return;
        }
        else
        {
            throw null;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    public virtual void Print(SourceProductionContext context, CodeBuilder builder) => throw null;
}
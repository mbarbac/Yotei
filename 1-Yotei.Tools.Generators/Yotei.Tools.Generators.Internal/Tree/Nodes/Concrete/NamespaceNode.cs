namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal class NamespaceNode : INode
{
    INode Initialize(INode parent)
    {
        parent.ThrowWhenNull();

        if (parent is not FileNode and not NamespaceNode) throw new ArgumentException(
            "Parent node is not a file or namespace one.")
            .WithData(parent);

        return parent;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    public NamespaceNode(INode parent, string name)
    {
        ParentNode = Initialize(parent);
        Name = name.NotNullNotEmpty();

        NamespaceChildren = new(this);
        TypeChildren = new(this);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    public NamespaceNode(INode parent, BaseNamespaceDeclarationSyntax syntax)
    {
        ParentNode = Initialize(parent);
        Syntax = syntax.ThrowWhenNull();
        Name = syntax.Name.ToString();

        NamespaceChildren = new(this);
        TypeChildren = new(this);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <inheritdoc/>
    public Hierarchy Hierarchy => ParentNode.Hierarchy;

    /// <summary>
    /// The parent node of this instance, it being either a file one or a parent namespace one.
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The name of this namespace, including dots if it is a multipart one.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The syntax of this instance, or null if not created through a generator.
    /// </summary>
    public BaseNamespaceDeclarationSyntax? Syntax { get; }

    /// <summary>
    /// The collection of namespace nodes registered into this instance.
    /// </summary>
    public NamespaceChildrenList NamespaceChildren { get; }
    public class NamespaceChildrenList : ChildrenList<NamespaceNode>
    {
        public NamespaceChildrenList(NamespaceNode master) : base(master)
        {
            Compare = (x, y) => x.Name == y.Name;
        }
        protected override string ItemToString(NamespaceNode item) => item.Name;
    }

    /// <summary>
    /// The collection of type nodes registered into this instance.
    /// </summary>
    public TypeChildrenList TypeChildren { get; }
    public class TypeChildrenList : ChildrenList<TypeNode>
    {
        public TypeChildrenList(NamespaceNode master) : base(master)
        {
            Compare = (x, y) => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
        }
        protected override string ItemToString(TypeNode item) => item.Symbol.Name;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in NamespaceChildren) if (!node.Validate(context)) return false;
        foreach (var node in TypeChildren) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"namespace {Name}");
        cb.AppendLine("{");
        cb.IndentLevel++;

        Printusings(cb);
        foreach (var node in NamespaceChildren) node.Print(context, cb);
        foreach (var node in TypeChildren) node.Print(context, cb);

        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Emits the file-level usings.
    /// </summary>
    /// <param name="cb"></param>
    void Printusings(CodeBuilder cb)
    {
        var items = new CustomList<string>()
        {
            Compare = (x, y) => x == y,
            CanInclude = (item, x) => !ReferenceEquals(item, x),
        };

        if (Syntax is not null)
        {
            foreach (var item in Syntax.Usings)
            {
                var str = item.ToString();
                if (str != null && str.Length > 0) items.Add(str);
            }
        }

        if (items.Count > 0)
        {
            foreach (var item in items) cb.AppendLine(item);
            cb.AppendLine();
        }
    }
}
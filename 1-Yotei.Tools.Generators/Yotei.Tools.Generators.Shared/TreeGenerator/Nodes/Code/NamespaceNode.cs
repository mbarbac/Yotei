namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal class NamespaceNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    public NamespaceNode(INode parent, string name)
    {
        ParentNode = parent.ThrowWhenNull();
        Name = name.NotNullNotEmpty();

        if (parent is not FileBuilder and not NamespaceNode) throw new ArgumentException(
            "Parent is not a file node nor a namespace node.")
            .WithData(parent);

        NamespaceChildren = new(this);
        TypeChildren = new(this);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    public NamespaceNode(INode parent, BaseNamespaceDeclarationSyntax syntax)
        : this(parent, syntax.Name.ToString())
        => Syntax = syntax.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Namespace: {Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseGenerator Generator => ParentNode.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The name of this namespace, including dots if it is a multipart on.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The syntax of this node, or null if not created by a generator.
    /// </summary>
    public BaseNamespaceDeclarationSyntax? Syntax { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of namespace nodes registered into this instance.
    /// </summary>
    public NamespaceList NamespaceChildren { get; }
    public class NamespaceList : ChildrenList<NamespaceNode>
    {
        public NamespaceList(NamespaceNode master)
            : base(master)
            => OnCompare = (item, other) => item.Name == other.Name;
    }

    /// <summary>
    /// The collection of type nodes registered into this instance.
    /// </summary>
    public TypeList TypeChildren { get; }
    public class TypeList : ChildrenList<TypeNode>
    {
        public TypeList(NamespaceNode master)
            : base(master)
            => OnCompare = (item, other)
            => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in NamespaceChildren) if (!node.Validate(context)) return false;
        foreach (var node in TypeChildren) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    public void Print(SourceProductionContext context, CodeBuilder builder)
    {
        builder.AppendLine($"namespace {Name}");
        builder.AppendLine("{");
        builder.IndentLevel++;

        PrintUsings(builder);
        foreach (var node in NamespaceChildren) node.Print(context, builder);
        foreach (var node in TypeChildren) node.Print(context, builder);

        builder.IndentLevel--;
        builder.AppendLine("}");
    }

    /// <summary>
    /// Emits the namespace-level usings.
    /// </summary>
    /// <param name="builder"></param>
    void PrintUsings(CodeBuilder builder)
    {
        var list = new CustomList<string>()
        {
            OnAcceptDuplicate = (x, y) => false,
            OnCompare = (x, y) => x == y,
        };

        if (Syntax != null)
        {
            foreach (var item in Syntax.Usings)
            {
                var str = item.ToString();
                if (str != null && str.Length > 0) list.Add(str);
            }
        }

        if (list.Count > 0)
        {
            foreach (var item in list) builder.AppendLine(item);
            builder.AppendLine();
        }
    }
}
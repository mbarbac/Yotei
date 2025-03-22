namespace Yotei.Tools.BaseGenerator;

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
    /// <inheritdoc/>
    /// <br/> The value of this property is either a parent file, or a parent namespace.
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

    // ----------------------------------------------------

    /// <inheritdoc/>
    public Compilation GetHierarchyCompilation() => ParentNode.GetHierarchyCompilation();

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces registered in this namespace node.
    /// </summary>
    public ChildNamespaces ChildNamespaces { get; } = [];

    /// <summary>
    /// The collection of child types registered in this namespace node.
    /// </summary>
    public ChildTypes ChildTypes { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context)
    {
        var r = true;

        foreach (var node in ChildNamespaces) if (!node.Validate(context)) r = false;
        foreach (var node in ChildTypes) if (!node.Validate(context)) r = false;

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"namespace {Name}");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            GenerateUsings(cb);

            var done = false;

            foreach (var node in ChildNamespaces)
            {
                if (done) cb.AppendLine(); done = true;
                node.Emit(context, cb);
            }
            
            foreach (var node in ChildTypes)
            {
                if (done) cb.AppendLine(); done = true;
                node.Emit(context, cb);
            }
        }
        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Generates namespace-level usings, if any.
    /// </summary>
    /// <param name="cb"></param>
    void GenerateUsings(CodeBuilder cb)
    {
        var items = new List<string>();

        foreach (var item in Syntax.Usings)
        {
            var str = item.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(str) && !items.Contains(str)) items.Add(str);
        }

        if (items.Count > 0)
        {
            foreach (var item in items) cb.AppendLine(item);
            cb.AppendLine();
        }
    }
}
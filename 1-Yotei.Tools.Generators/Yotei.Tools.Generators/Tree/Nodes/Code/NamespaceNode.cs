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
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in ChildNamespaces) if (!node.Validate(context)) return false;
        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        return true;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
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

    /// <summary>
    /// Generates namespace-level usings...
    /// </summary>
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
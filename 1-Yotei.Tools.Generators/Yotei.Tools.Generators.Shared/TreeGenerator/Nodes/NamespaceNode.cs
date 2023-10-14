namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal partial class NamespaceNode : Node
{
    /// <summary>
    /// Initializes a new instance, using the given syntax node.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    public NamespaceNode(Node parent, BaseNamespaceDeclarationSyntax syntax) : base(
        parent!.Generator ??
        throw new ArgumentNullException(nameof(parent)))
    {
        Parent = ValidateParent(parent);
        Syntax = syntax.ThrowWhenNull(nameof(syntax));
        LongName = Syntax.Name.LongName().NotNullNotEmpty(nameof(Syntax.Name));

        ChildNamespaces = new();
        ChildTypes = new();
    }

    static Node ValidateParent(Node parent)
    {
        if (parent is not FileNode and not NamespaceNode)
            throw new ArgumentException(
                "Parent is not a file or namespace node.")
                .WithData(parent, nameof(parent));

        return parent;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Namespace: {LongName}";

    /// <summary>
    /// The node in the hierarchy that contains this instance.
    /// </summary>
    public Node Parent { get; }

    /// <summary>
    /// The long name of this namespace, including its dots if needed.
    /// </summary>
    public string LongName { get; }

    /// <summary>
    /// The syntax associated with this instance, or null if this information is not available.
    /// </summary>
    public BaseNamespaceDeclarationSyntax? Syntax { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child namespaces registered into this instance.
    /// </summary>
    public ChildNamespaces ChildNamespaces { get; }

    /// <summary>
    /// The collection of child types registered into this instance.
    /// </summary>
    public ChildTypes ChildTypes { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the contents of this instance, and its child elements, if any.
    /// Returns true if it is valid for source generation purposes, of false otherwise.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        foreach (var node in ChildNamespaces) if (!node.Validate(context)) return false;
        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to append to the given code builder the contents of this instance, and its
    /// child elements, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"namespace {LongName}");
        cb.AppendLine("{");
        cb.IndentLevel++;

        PrintUsings(cb);
        var num = 0;

        foreach (var node in ChildNamespaces)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }

        foreach (var node in ChildTypes)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to print the namespace-level usings.
    /// </summary>
    /// <param name="cb"></param>
    protected virtual void PrintUsings(CodeBuilder cb)
    {
        var list = new NoDuplicatesList<string>()
        {
            ThrowDuplicates = false,
            Equivalent = (x, y) => x == y
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
            foreach (var item in list) cb.AppendLine(item);
            cb.AppendLine();
        }
    }
}
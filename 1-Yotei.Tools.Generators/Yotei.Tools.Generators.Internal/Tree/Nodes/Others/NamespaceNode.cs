namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace in the source code generation hierarchy.
/// </summary>
/// 
internal sealed class NamespaceNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="syntaxNode"></param>
    public NamespaceNode(INode parentNode, BaseNamespaceDeclarationSyntax syntaxNode)
    {
        ParentNode = parentNode.ThrowWhenNull();
        Syntax = syntaxNode.ThrowWhenNull();

        if (ParentNode is not FileNode and not NamespaceNode)
            throw new ArgumentException(
                "Parent node is not a file nor a namespace.")
                .WithData(parentNode);
    }

    /// <inheritdoc/>
    public INode ParentNode { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// The syntax this instance represents.
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; }

    /// <summary>
    /// The name of this namespace, including dot separated parts if they are explicitly used.
    /// </summary>
    public string Name => Syntax.Name.ToString();

    /// <summary>
    /// The list of child namespaces.
    /// <br/> Default equality: long namespace name, with its dot separated parts.
    /// </summary>
    public List<NamespaceNode> ChildNamespaces { get; } = [];

    /// <summary>
    /// The list of child types.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<TypeNode> ChildTypes { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in ChildNamespaces) if (!node.Validate(context)) return false;
        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"namespace {Name}");
        cb.AppendLine("{");
        cb.IndentLevel++;

        PrintUsings(cb);

        foreach (var node in ChildNamespaces) node.Emit(context, cb);
        foreach (var node in ChildTypes) node.Emit(context, cb);

        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Emits the namespace-level usings.
    /// </summary>
    void PrintUsings(CodeBuilder cb)
    {
        var items = new List<string>();

        foreach (var item in Syntax.Usings)
        {
            var str = item.ToString();
            if (!string.IsNullOrWhiteSpace(str) && !items.Contains(str)) items.Add(str);
        }
        if (items.Count > 0)
        {
            foreach (var item in items) cb.AppendLine(item);
            cb.AppendLine();
        }
    }
}
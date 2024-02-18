namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace in the source code generation hierarchy.
/// </summary>
/// <param name="node"></param>
internal sealed class NamespaceNode(BaseNamespaceDeclarationSyntax node) : INode
{
    /// <inheritdoc/>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// The syntax this instance represents.
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; } = node.ThrowWhenNull();

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
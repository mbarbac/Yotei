namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// <inheritdoc cref="INamespaceNode"/>
/// </summary>
internal class NamespaceNode : INamespaceNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    public NamespaceNode(
        INode parent, BaseNamespaceDeclarationSyntax syntax)
    {
        Parent =
            parent is IFileNode file ? file :
            parent is INamespaceNode ns ? ns :
            parent is null ? throw new ArgumentNullException(nameof(parent)) :
            throw new ArgumentException($"Invalid parent node: {parent}");

        Syntax = syntax.ThrowIfNull(nameof(syntax));
        Name = syntax.Name.LongName();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IGenerator Generator => Parent.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode Parent { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseNamespaceDeclarationSyntax Syntax { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<INamespaceNode> ChildNamespaces => _ChildNamespaces;
    readonly List<INamespaceNode> _ChildNamespaces = new();

    INamespaceNode LocateNamespace(ICaptured captured, int nsIndex)
    {
        var type = captured.AsCapturedType();
        var syntax = type.NamespaceSyntaxChain[nsIndex];
        var name = syntax.Name.LongName();

        var node = _ChildNamespaces.Find(x => x.Name == name);
        if (node == null)
        {
            node = new NamespaceNode(this, syntax);
            _ChildNamespaces.Add(node);
        }
        return node;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<ITypeNode> ChildTypes => _ChildTypes;
    readonly List<ITypeNode> _ChildTypes = new();

    ITypeNode LocateType(ICaptured captured, int tpIndex)
    {
        var type = captured.AsCapturedType();
        var syntax = type.TypeSyntaxChain[tpIndex];
        var symbol = type.TypeSymbolChain[tpIndex];
        var name = symbol.ShortName();
        var len = type.TypeSyntaxChain.Length;

        var node = _ChildTypes.Find(x => x.Name == name);
        if (node == null)
        {
            node = tpIndex < (len - 1)
                ? new TypeNode(this, syntax, symbol, captured.SemanticModel)
                : captured.Generator.CreateType(this, syntax, symbol, captured.SemanticModel);

            _ChildTypes.Add(node);
        }
        return node;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Register(ICaptured captured, int nsIndex, int tpIndex)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var type = captured.AsCapturedType();
        var nsLen = type.NamespaceSyntaxChain.Length;

        // Child Namespace...
        if (nsIndex < nsLen)
        {
            var node = LocateNamespace(captured, nsIndex);
            node.Register(captured, nsIndex + 1, tpIndex);
        }

        // Child Type...
        else
        {
            var node = LocateType(captured, tpIndex);
            node.Register(captured, tpIndex + 1);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in ChildNamespaces) if (!node.Validate(context)) return false;
        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"namespace {Name}");
        cb.AppendLine("{");
        cb.Tabs++;
        PrintUsings(cb);

        var num = 0;

        foreach (var node in ChildNamespaces)
        {
            if (num > 0) cb.AppendLine(); num++;
            node.Print(context, cb);
        }

        foreach (var node in ChildTypes)
        {
            if (num > 0) cb.AppendLine(); num++;
            node.Print(context, cb);
        }

        cb.Tabs--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Appends namespace-level usings.
    /// </summary>
    /// <param name="cb"></param>
    void PrintUsings(CodeBuilder cb)
    {
        var num = 0;
        foreach (var item in Syntax.Usings)
        {
            cb.AppendLine($"using {item.Name};");
            num++;
        }
        if (num > 0) cb.AppendLine();
    }
}
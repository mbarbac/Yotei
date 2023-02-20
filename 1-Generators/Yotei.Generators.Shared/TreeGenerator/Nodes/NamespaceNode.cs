namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a namespace in the hierarchy.
/// </summary>
internal class NamespaceNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="captured"></param>
    /// <param name="index"></param>
    public NamespaceNode(ICapturedType captured, int index)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        var nsSyntax = captured.NamespaceSyntaxChain[index];
        Name = nsSyntax.Name.LongName();
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"Namespace: {Name}";

    /// <summary>
    /// The long namespace name of this instance.
    /// </summary>
    public string Name { get; }

    // ----------------------------------------------------
    /// <summary>
    /// The collection of child namespace nodes.
    /// </summary>
    List<NamespaceNode> ChildNamespaces { get; } = new();

    NamespaceNode LocateChildNamespace(ICaptured captured, int nsIndex)
    {
        var item = captured.AsCapturedType();
        var nsSyntax = item.NamespaceSyntaxChain[nsIndex];
        var nsName = nsSyntax.Name.LongName();

        var node = ChildNamespaces.Find(x => x.Name == nsName);
        if (node == null)
        {
            node = new NamespaceNode(item, nsIndex);
            ChildNamespaces.Add(node);
        }
        return node;
    }

    /// <summary>
    /// The collection of child type nodes.
    /// </summary>
    List<TypeNode> ChildTypeNodes { get; } = new();

    TypeNode LocateChildTypeNode(ICaptured captured, int index)
    {
        var item = captured.AsCapturedType();
        var tpSyntax = item.TypeSyntaxChain[index];
        var tpSymbol = item.TypeSymbolChain[index];
        var tpName = tpSymbol.ShortName();

        var node = ChildTypeNodes.Find(x => x.Name == tpName);
        if (node == null)
        {
            node = new TypeNode(item, index);
            ChildTypeNodes.Add(node);
        }
        return node;
    }

    /// <summary>
    /// The collection of child captured types.
    /// </summary>
    List<ICapturedType> ChildCapturedTypes { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register into this instance the given captured element, at the level given
    /// by the namespace and type indexes.
    /// </summary>
    public void Register(ICaptured captured, int nsIndex, int tpIndex)
    {
        var item = captured.AsCapturedType();
        var nsLen = item.NamespaceSyntaxChain.Length;
        var tpLen = item.TypeSyntaxChain.Length;
        var level = item.Generator.CaptureLevel;

        // Namespace...        
        if (nsIndex < nsLen)
        {
            var node = LocateChildNamespace(captured, nsIndex);
            node.Register(captured, nsIndex + 1, tpIndex);
            return;
        }

        // Terminal Type...
        if (tpIndex == (tpLen - 1) && level == CaptureLevel.Type)
        {
            var node = ChildCapturedTypes.Find(x => x.Name == item.Name);
            if (node == null) ChildCapturedTypes.Add(item);
            return;
        }

        // Intermediate type...
        else
        {
            var node = LocateChildTypeNode(captured, tpIndex);
            node.Register(captured, tpIndex + 1);
            return;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this instance and its contents, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var nsNode in ChildNamespaces) if (!nsNode.Validate(context)) return false;
        foreach (var tpNode in ChildTypeNodes) if (!tpNode.Validate(context)) return false;
        foreach (var tpItem in ChildCapturedTypes) if (!tpItem.Validate(context)) return false;
        return true;
    }

    /// <summary>
    /// Invoked to generate the source code for this instance and its contents, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"namespace {Name}");
        cb.AppendLine("{");
        cb.Tabs++;

        var num = 0;

        for (int i = 0; i < ChildNamespaces.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildNamespaces[i].Print(context, cb);
        }

        for (int i = 0; i < ChildTypeNodes.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildTypeNodes[i].Print(context, cb);
        }

        for (int i = 0; i < ChildCapturedTypes.Count; i++)
        {
            if (num > 0) cb.AppendLine();
            num++;
            ChildCapturedTypes[i].Print(context, cb);
        }

        cb.Tabs--;
        cb.AppendLine("}");
    }
}
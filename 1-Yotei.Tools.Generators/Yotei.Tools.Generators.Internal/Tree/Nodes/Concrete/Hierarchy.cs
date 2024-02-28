namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents the hierarchy of elements created by its associated generator.
/// </summary>
/// <param name="generator"></param>
internal class Hierarchy(TreeGenerator generator) : INode
{
    /// <summary>
    /// The generator that has created this hierarchy.
    /// </summary>
    public TreeGenerator Generator { get; } = generator.ThrowWhenNull();

    /// <summary>
    /// The child files registered into this instance.
    /// </summary>
    public ChildList<FileNode> ChildFiles { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register the given candidate into this hierarchy.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="candidate"></param>
    public void Register(SourceProductionContext context, ICandidate candidate)
    {
        // File level...
        var fname = Generator.GetFileName(candidate);
        var fnode = ChildFiles.Find(x => x.FileName == fname);
        if (fnode == null)
        {
            fnode = new(this, fname);
            ChildFiles.Add(fnode);
        }

        var comparer = SymbolEqualityComparer.Default;
        INode parent = fnode;
        int len;

        // Namespace level...
        NamespaceNode nsnode = default!;
        var nslist = fnode.ChildNamespaces;

        len = candidate.NamespaceSyntaxChain.Length;
        for (int nsindex = 0; nsindex < len; nsindex++)
        {
            var syntax = candidate.NamespaceSyntaxChain[nsindex];

            nsnode = nslist.Find(x => ReferenceEquals(x.Syntax, syntax))!;
            if (nsnode == null)
            {
                nsnode = new(parent, syntax);
                nslist.Add(nsnode);
            }

            parent = nsnode;
            nslist = nsnode.ChildNamespaces;
        }

        // Type level...
        TypeNode tpnode = default!;
        var tplist = nsnode.ChildTypes;

        len = candidate.TypeSymbolChain.Length;
        for (int tpindex = 0; tpindex < len; tpindex++)
        {
            var symbol = candidate.TypeSymbolChain[tpindex];

            tpnode = tplist.Find(x => ReferenceEquals(x.Symbol, symbol))!;
            if (tpnode == null)
            {
                if (candidate is TypeCandidate typeCandidate && tpindex == (len - 1))
                {
                    tpnode = Generator.CreateNode(parent, typeCandidate);
                    tplist.Add(tpnode);
                }
                else // A type created ad-hoc for hierarchy purposes only...
                {
                    tpnode = new TypeNode(parent, symbol);
                    tplist.Add(tpnode);
                }
            }

            // We have a previous node but we need to register a candidate...
            else
            {
                if (candidate is TypeCandidate typeCandidate && tpindex == (len - 1))
                {
                    context.DuplicatedHierarchyType(typeCandidate.Syntax);
                    context.DuplicatedHierarchyType(typeCandidate.Symbol, true);
                }
            }

            parent = tpnode;
            tplist = tpnode.ChildTypes;
        }

        // Ending type level...
        if (candidate is TypeCandidate) return;

        // Property level...
        if (candidate is PropertyCandidate propertyCandidate)
        {
            var temp = tpnode.ChildProperties.Find(
                x => ReferenceEquals(x.Symbol, propertyCandidate.Symbol));

            if (temp == null)
            {
                var node = Generator.CreateNode(tpnode, propertyCandidate);
                tpnode.ChildProperties.Add(node);
            }
            return;
        }

        // Field level...
        if (candidate is FieldCandidate FieldCandidate)
        {
            var temp = tpnode.ChildFields.Find(
                x => ReferenceEquals(x.Symbol, FieldCandidate.Symbol));

            if (temp == null)
            {
                var node = Generator.CreateNode(tpnode, FieldCandidate);
                tpnode.ChildFields.Add(node);
            }
            return;
        }

        // Method level...
        if (candidate is MethodCandidate methodCandidate)
        {
            var temp = tpnode.ChildMethods.Find(
                x => ReferenceEquals(x.Symbol, methodCandidate.Symbol));

            if (temp == null)
            {
                var node = Generator.CreateNode(tpnode, methodCandidate);
                tpnode.ChildMethods.Add(node);
            }
            return;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds in this hierarchy the first node that matches the given predicate, or null if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IChildNode? Find(Predicate<IChildNode> predicate)
    {
        predicate.ThrowWhenNull();

        foreach (var item in ChildFiles)
        {
            var temp = Find(item, predicate);
            if (temp != null) return temp;
        }
        return null;
    }

    static IChildNode? Find(FileNode node, Predicate<IChildNode> predicate)
    {
        if (predicate(node)) return node;

        foreach (var item in node.ChildNamespaces)
        {
            var temp = Find(item, predicate);
            if (temp != null) return temp;
        }
        return null;
    }

    static IChildNode? Find(NamespaceNode node, Predicate<IChildNode> predicate)
    {
        if (predicate(node)) return node;
        
        foreach (var item in node.ChildNamespaces)
        {
            var temp = Find(item, predicate);
            if (temp != null) return temp;
        }
        foreach (var item in node.ChildTypes)
        {
            var temp = Find(item, predicate);
            if (temp != null) return temp;
        }
        return null;
    }

    static IChildNode? Find(TypeNode node, Predicate<IChildNode> predicate)
    {
        if (predicate(node)) return node;

        foreach (var item in node.ChildTypes)
        {
            var temp = Find(item, predicate);
            if (temp != null) return temp;
        }
        foreach (var item in node.ChildProperties)
        {
            if (predicate(item)) return item;
        }
        foreach (var item in node.ChildFields)
        {
            if (predicate(item)) return item;
        }
        foreach (var item in node.ChildMethods)
        {
            if (predicate(item)) return item;
        }
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Finds in this hierarchy all the nodes that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public ChildList<IChildNode> FindAll(Predicate<IChildNode> predicate)
    {
        predicate.ThrowWhenNull();
        ChildList<IChildNode> list = [];

        foreach (var item in ChildFiles)
        {
            var temp = FindAll(item, predicate);
            list.AddRange(temp, raiseDuplicates: false);
        }
        return list;
    }

    static ChildList<IChildNode> FindAll(FileNode node, Predicate<IChildNode> predicate)
    {
        ChildList<IChildNode> list = [];
        if (predicate(node)) list.Add(node);

        foreach (var item in node.ChildNamespaces)
        {
            var temp = FindAll(item, predicate);
            list.AddRange(temp, raiseDuplicates: false);
        }
        return list;
    }

    static ChildList<IChildNode> FindAll(NamespaceNode node, Predicate<IChildNode> predicate)
    {
        ChildList<IChildNode> list = [];
        if (predicate(node)) list.Add(node);

        foreach (var item in node.ChildNamespaces)
        {
            var temp = FindAll(item, predicate);
            list.AddRange(temp, raiseDuplicates: false);
        }
        foreach (var item in node.ChildTypes)
        {
            var temp = FindAll(item, predicate);
            list.AddRange(temp, raiseDuplicates: false);
        }
        return list;
    }

    static ChildList<IChildNode> FindAll(TypeNode node, Predicate<IChildNode> predicate)
    {
        ChildList<IChildNode> list = [];
        if (predicate(node)) list.Add(node);

        foreach (var item in node.ChildTypes)
        {
            var temp = FindAll(item, predicate);
            list.AddRange(temp, raiseDuplicates: false);
        }
        foreach (var item in node.ChildProperties)
        {
            if (predicate(item)) list.Add(item, raiseDuplicates: false);
        }
        foreach (var item in node.ChildFields)
        {
            if (predicate(item)) list.Add(item, raiseDuplicates: false);
        }
        foreach (var item in node.ChildMethods)
        {
            if (predicate(item)) list.Add(item, raiseDuplicates: false);
        }
        return list;
    }
}
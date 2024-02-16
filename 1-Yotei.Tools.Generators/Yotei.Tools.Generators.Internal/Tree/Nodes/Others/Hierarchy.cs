namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a source code generation hiearchy.
/// </summary>
internal sealed class Hierarchy
{
    /// <summary>
    /// The list of child files.
    /// </summary>
    public List<FileNode> ChildFiles { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to register the type candidates.
    /// </summary>
    /// <param name="candidates"></param>
    public void RegisterTypeCandidates(ImmutableArray<ICandidate> candidates)
    {
        foreach (var candidate in candidates)
            if (candidate is ITypeCandidate) RegisterCandidate(candidate);
    }

    /// <summary>
    /// Invoked to register candidates that are not type ones.
    /// </summary>
    /// <param name="candidates"></param>
    public void RegisterOtherCandidates(ImmutableArray<ICandidate> candidates)
    {
        foreach (var candidate in candidates)
            if (candidate is not ITypeCandidate) RegisterCandidate(candidate);
    }

    /// <summary>
    /// Invoked to register a given candidate.
    /// </summary>
    /// <param name="candidate"></param>
    void RegisterCandidate(ICandidate candidate)
    {
        var comparer = SymbolEqualityComparer.Default;

        // File...
        var fname = candidate.GetFileName();
        var fnode = ChildFiles.Find(x => x.FileName == fname);
        if (fnode == null)
        {
            fnode = new(fname);
            ChildFiles.Add(fnode);
        }

        // Namespaces...
        NamespaceNode nsnode = default!;
        var nsList = fnode.ChildNamespaces;
        for (int nsindex = 0; nsindex < candidate.NamespaceSyntaxChain.Length; nsindex++)
        {
            var syntax = candidate.NamespaceSyntaxChain[nsindex];
            var name = syntax.Name.ToString();

            nsnode = nsList.Find(x => x.Name == name);
            if (nsnode == null)
            {
                nsnode = new NamespaceNode(syntax);
                nsList.Add(nsnode);
            }

            nsList = nsnode.ChildNamespaces;
        }

        // Types...
        TypeNode tpnode = default!;
        var tpList = nsnode.ChildTypes;
        for (int tpindex = 0; tpindex < candidate.TypeSymbolChain.Length; tpindex++)
        {
            var syntax = candidate.TypeSyntaxChain[tpindex];
            var symbol = candidate.TypeSymbolChain[tpindex];

            tpnode = tpList.Find(x => comparer.Equals(symbol, x.Symbol));
            if (tpnode == null)
            {
                if (candidate is TypeNode temp && tpindex == (candidate.TypeSymbolChain.Length - 1))
                {
                    tpList.Add(temp);
                    return;
                }
                else
                {
                    tpnode = new TypeNode(candidate.SemanticModel, syntax, symbol);
                    tpList.Add(tpnode);
                }
            }

            tpList = tpnode.ChildTypes;
        }

        // Properties...
        if (candidate is PropertyNode propertyNode)
        {
            var temp = tpnode.ChildProperties.Find(x => comparer.Equals(x.Symbol, candidate.Symbol));
            if (temp == null) tpnode.ChildProperties.Add(propertyNode);
            return;
        }

        // Fields...
        if (candidate is FieldNode fieldNode)
        {
            var temp = tpnode.ChildFields.Find(x => comparer.Equals(x.Symbol, candidate.Symbol));
            if (temp == null) tpnode.ChildFields.Add(fieldNode);
            return;
        }

        // Methods...
        if (candidate is MethodNode methodNode)
        {
            var temp = tpnode.ChildMethods.Find(x => comparer.Equals(x.Symbol, candidate.Symbol));
            if (temp == null) tpnode.ChildMethods.Add(methodNode);
            return;
        }

        // Unknown...
        throw new InvalidOperationException("Unknown candidate type.").WithData(candidate);
    }
}
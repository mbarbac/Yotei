namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike node in the source code generation hierarchy.
/// </summary>
internal class PropertyNode : IChildNode
{
    // <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public PropertyNode(TypeNode parent, IPropertySymbol symbol)
    {
        ParentNode = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public PropertyNode(TypeNode parent, PropertyCandidate candidate)
        : this(parent, candidate.ThrowWhenNull().Symbol) => Candidate = candidate;

    /// <inheritdoc/>
    public override string ToString()
    {
        var options = RoslynNameOptions.Default with { UseMemberHost = RoslynNameOptions.Default };
        return Symbol.EasyName(options);
    }

    /// <summary>
    /// The type node this instance belongs to, in the source code generation hierarchy.
    /// </summary>
    public TypeNode ParentNode { get; }
    INode IChildNode.ParentNode => ParentNode;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IPropertySymbol Symbol { get; }

    /// <summary>
    /// The candidate from which this instance was obtained, or null if not available.
    /// </summary>
    public PropertyCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public Compilation GetHierarchyCompilation() => ParentNode.GetHierarchyCompilation();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (!ParentNode.Symbol.IsPartial())
        {
            TreeDiagnostics.TypeIsNotPartial(ParentNode.Symbol).Report(context);
            r = false;
        }

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
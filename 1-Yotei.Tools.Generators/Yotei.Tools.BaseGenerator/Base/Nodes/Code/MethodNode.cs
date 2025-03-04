namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : IChildNode
{
    // <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public MethodNode(TypeNode parent, IMethodSymbol symbol)
    {
        ParentNode = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public MethodNode(TypeNode parent, MethodCandidate candidate)
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
    public IMethodSymbol Symbol { get; }

    /// <summary>
    /// The candidate from which this instance was obtained, or null if not available.
    /// </summary>
    public MethodCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context)
    {
        if (!ParentNode.Symbol.IsPartial())
        {
            TreeDiagnostics.TypeIsNotPartial(ParentNode.Symbol).Report(context);
            return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
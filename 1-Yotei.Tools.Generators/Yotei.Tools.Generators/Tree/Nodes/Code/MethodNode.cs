namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : IChildNode
{
    /// <summary>
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
        : this(parent, candidate.ThrowWhenNull().Symbol)
        => Candidate = candidate;

    /// <inheritdoc/>
    public override string ToString() => $"Method: {Symbol.Name}";

    /// <summary>
    /// The type-alike node this instance belongs to in the source code generation hierarchy.
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

    // -----------------------------------------------------

    /// <summary>
    /// Invoked before generation to validate this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context)
    {
        if (!ParentNode.Symbol.IsPartial())
        {
            context.ReportDiagnostic(TreeDiagnostics.TypeIsNotPartial(ParentNode.Symbol));
            return false;
        }

        return true;
    }

    // -----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
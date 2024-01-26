namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : INode
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
    {
        ParentNode = parent.ThrowWhenNull();
        Candidate = candidate.ThrowWhenNull();
        Symbol = candidate.Symbol;
    }

    // <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var same = SymbolEqualityComparer.Default.Equals(
            Symbol.ContainingType,
            ParentNode.Symbol);

        return same
            ? $"Method: {Symbol.EasyName()}"
            : $"Method({ParentNode.Symbol.Name}): {Symbol.EasyName()}";
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Hierarchy Hierarchy => ParentNode.Hierarchy;

    /// <summary>
    /// The parent node of this instance, it being either a namespace or a parent type one.
    /// </summary>
    public TypeNode ParentNode { get; }
    INode? INode.ParentNode => ParentNode;

    /// <summary>
    /// The symbol of the associated method.
    /// </summary>
    public IMethodSymbol Symbol { get; }

    /// <summary>
    /// The candidate of this instance, or null if not created through a generator.
    /// </summary>
    public MethodCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
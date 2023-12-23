namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a property-alike node in a source code generation hierarchy.
/// </summary>
internal class PropertyNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
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
        : this(parent, candidate.Symbol)
        => Candidate = candidate.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = $"Property: {Symbol.ToStringEx(useSymbolType: true)}";

        if (!SymbolEqualityComparer.Default.Equals(Symbol.ContainingType, ParentNode.Symbol))
            str += $" on: {ParentNode.Symbol.Name}";

        return str;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseGenerator Generator => ParentNode.Generator;

    /// <summary>
    /// The parent type node this instance belongs to.
    /// </summary>
    public TypeNode ParentNode { get; }
    INode? INode.ParentNode => ParentNode;

    // ----------------------------------------------------

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IPropertySymbol Symbol { get; }

    /// <summary>
    /// The candidate this instance is associated with, or null if this node has not been created
    /// by the generator.
    /// </summary>
    public PropertyCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    public virtual void Print(SourceProductionContext context, CodeBuilder builder) => throw null;
}
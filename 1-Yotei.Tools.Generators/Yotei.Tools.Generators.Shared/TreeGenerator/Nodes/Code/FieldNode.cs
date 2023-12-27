namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a field-alike node in the source code generation hierarchy.
/// </summary>
internal class FieldNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    [SuppressMessage("", "IDE0290")]
    public FieldNode(TypeNode parent, IFieldSymbol symbol)
    {
        ParentNode = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public FieldNode(TypeNode parent, FieldCandidate candidate)
        : this(parent, candidate.Symbol)
        => Candidate = candidate.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = $"Field: {Symbol.ToStringEx(useSymbolType: true)}";

        if (!SymbolEqualityComparer.Default.Equals(Symbol.ContainingType, ParentNode.Symbol))
            str += $" on: {ParentNode.Symbol.Name}";

        return str;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseGenerator Generator => ParentNode.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TypeNode ParentNode { get; }
    INode? INode.ParentNode => ParentNode;

    /// <summary>
    /// The symbol of this type.
    /// </summary>
    public IFieldSymbol Symbol { get; }

    /// <summary>
    /// The candidate of this node, or null if not created by a generator.
    /// </summary>
    public FieldCandidate? Candidate { get; }

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
    /// <param name="builder"></param>
    public virtual void Print(
        SourceProductionContext context, CodeBuilder builder)
        => builder.AppendLine($"// {ToString()}");
}
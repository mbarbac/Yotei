namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(INode parent, INamedTypeSymbol symbol)
    {
        ParentNode = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();

        if (ParentNode is not NamespaceNode and not TypeNode) throw new ArgumentException(
            "Parent node is not a namespace nor a type.")
            .WithData(parent);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public TypeNode(INode parent, TypeCandidate candidate)
        : this(parent, candidate.ThrowWhenNull().Symbol) => Candidate = candidate;

    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.EasyName()}";

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The value of this property is either a parent namespace or a parent type.
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The candidate from which this instance was obtained, or null if not available.
    /// </summary>
    public TypeCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child types registered in this type node.
    /// </summary>
    public ChildTypes ChildTypes { get; } = [];

    /// <summary>
    /// The collection of child properties registered in this type node.
    /// </summary>
    public ChildProperties ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields registered in this type node.
    /// </summary>
    public ChildFields ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods registered in this type node.
    /// </summary>
    public ChildMethods ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Emit(SourceProductionContext context, CodeBuilder cb) { }
}
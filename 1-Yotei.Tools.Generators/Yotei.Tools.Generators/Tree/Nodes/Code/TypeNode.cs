namespace Yotei.Tools.Generators.Internal;

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
        : this(parent, candidate.ThrowWhenNull().Symbol)
        => Candidate = candidate;

    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// The parent node this instance belongs to in the source code generation hierarchy, which
    /// can be either a namespace or a parent type.
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

    // -----------------------------------------------------

    /// <summary>
    /// The collection of child types registered into this node.
    /// </summary>
    public ChildTypes ChildTypes { get; } = [];

    /// <summary>
    /// The collection of child properties registered into this node.
    /// </summary>
    public ChildProperties ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields registered into this node.
    /// </summary>
    public ChildFields ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods registered into this node.
    /// </summary>
    public ChildMethods ChildMethods { get; } = [];

    // -----------------------------------------------------

    /// <summary>
    /// Invoked before generation to validate this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context) => throw null;

    // -----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}
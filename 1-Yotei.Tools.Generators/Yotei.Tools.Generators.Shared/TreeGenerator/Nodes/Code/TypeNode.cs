namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a type-alike node in a source code generation hierarchy.
/// </summary>
internal class TypeNode : INode
{
    /// <summary>
    /// Private constructor.
    /// </summary>
    TypeNode(INode parent, INamedTypeSymbol symbol, bool _)
    {
        ParentNode = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();

        TypeNodes = new TypeList(this);
        PropertyNodes = new PropertyList(this);
        FieldNodes = new FieldList(this);
        MethodNodes = new MethodList(this);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(NamespaceNode parent, INamedTypeSymbol symbol) : this(parent, symbol, false) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(TypeNode parent, INamedTypeSymbol symbol) : this(parent, symbol, false) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public TypeNode(NamespaceNode parent, TypeCandidate candidate)
        : this(parent, candidate.Symbol)
        => Candidate = candidate.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(TypeNode parent, TypeCandidate candidate)
        : this(parent, candidate.Symbol)
        => Candidate = candidate.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseGenerator Generator => ParentNode.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The namespace node this instance belong to, or null if this is not a top-most type.
    /// </summary>
    public NamespaceNode? ParentNamespaceNode => ParentNode as NamespaceNode;

    /// <summary>
    /// The type node this instance belong to, or null if this is a top-most type.
    /// </summary>
    public TypeNode? ParentTypeNode => ParentNode as TypeNode;

    // ----------------------------------------------------

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The candidate this instance is associated with, or null if this node has not been created
    /// by the generator.
    /// </summary>
    public TypeCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of type nodes registered in this instance.
    /// </summary>
    public TypeList TypeNodes { get; }
    public class TypeList : CustomList<TypeNode>
    {
        readonly TypeNode Master;
        public TypeList(TypeNode master)
        {
            Master = master;
            OnValidate = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                    "Parent node of the given item is not this instance.");
                return item;
            };
            OnCompare = (item, other) => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
            OnSameElement = ReferenceEquals;
            OnAcceptDuplicate = (item, other) => false;
        }
    }

    /// <summary>
    /// The collection of property nodes registered in this instance.
    /// </summary>
    public PropertyList PropertyNodes { get; }
    public class PropertyList : CustomList<PropertyNode>
    {
        readonly TypeNode Master;
        public PropertyList(TypeNode master)
        {
            Master = master;
            OnValidate = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                    "Parent node of the given item is not this instance.");
                return item;
            };
            OnCompare = (item, other) => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
            OnSameElement = ReferenceEquals;
            OnAcceptDuplicate = (item, other) => false;
        }
    }

    /// <summary>
    /// The collection of field nodes registered in this instance.
    /// </summary>
    public FieldList FieldNodes { get; }
    public class FieldList : CustomList<FieldNode>
    {
        readonly TypeNode Master;
        public FieldList(TypeNode master)
        {
            Master = master;
            OnValidate = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                    "Parent node of the given item is not this instance.");
                return item;
            };
            OnCompare = (item, other) => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
            OnSameElement = ReferenceEquals;
            OnAcceptDuplicate = (item, other) => false;
        }
    }

    /// <summary>
    /// The collection of methods nodes registered in this instance.
    /// </summary>
    public MethodList MethodNodes { get; }
    public class MethodList : CustomList<MethodNode>
    {
        readonly TypeNode Master;
        public MethodList(TypeNode master)
        {
            Master = master;
            OnValidate = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add && !ReferenceEquals(Master, item.ParentNode)) throw new ArgumentException(
                    "Parent node of the given item is not this instance.");
                return item;
            };
            OnCompare = (item, other) => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
            OnSameElement = ReferenceEquals;
            OnAcceptDuplicate = (item, other) => false;
        }
    }
    
    // ----------------------------------------------------

    /// <summary>
    /// Registers the given candidate.
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="tpIndex"></param>
    public void Register(Candidate candidate, int tpIndex)
    {
        throw null;
    }

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
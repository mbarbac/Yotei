namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : INode
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

        if (parent is not NamespaceNode and not TypeNode) throw new ArgumentException(
            "Parent is not a namespace node nor a type node.")
            .WithData(parent);

        TypeChildren = new(this);
        PropertyChildren = new(this);
        FieldChildren = new(this);
        MethodChildren = new(this);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public TypeNode(INode parent, TypeCandidate candidate)
        : this(parent, candidate.Symbol)
        => Candidate = candidate.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BaseGenerator Generator => ParentNode.Generator;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The symbol of this type.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The candidate of this node, or null if not created by a generator.
    /// </summary>
    public TypeCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of type nodes registered into this instance.
    /// </summary>
    public TypeList TypeChildren { get; }
    public class TypeList : ChildrenList<TypeNode>
    {
        public TypeList(TypeNode master)
            : base(master)
            => OnCompare = (item, other)
            => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
    }

    /// <summary>
    /// The collection of property nodes registered into this instance.
    /// </summary>
    public PropertyList PropertyChildren { get; }
    public class PropertyList : ChildrenList<PropertyNode>
    {
        public PropertyList(TypeNode master)
            : base(master)
            => OnCompare = (item, other)
            => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
    }

    /// <summary>
    /// The collection of field nodes registered into this instance.
    /// </summary>
    public FieldList FieldChildren { get; }
    public class FieldList : ChildrenList<FieldNode>
    {
        public FieldList(TypeNode master)
            : base(master)
            => OnCompare = (item, other)
            => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
    }

    /// <summary>
    /// The collection of method nodes registered into this instance.
    /// </summary>
    public MethodList MethodChildren { get; }
    public class MethodList : ChildrenList<MethodNode>
    {
        public MethodList(TypeNode master)
            : base(master)
            => OnCompare = (item, other)
            => SymbolEqualityComparer.Default.Equals(item.Symbol, other.Symbol);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context)
    {
        if (!OnValidate(context)) return false;

        foreach (var node in TypeChildren) if (!node.Validate(context)) return false;
        foreach (var node in PropertyChildren) if (!node.Validate(context)) return false;
        foreach (var node in FieldChildren) if (!node.Validate(context)) return false;
        foreach (var node in MethodChildren) if (!node.Validate(context)) return false;

        return true;
    }

    /// <summary>
    /// Invoked to validate this node.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        if (!context.TypeIsPartial(Symbol)) return false;
        if (!context.TypeIsSupported(Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    public void Print(SourceProductionContext context, CodeBuilder builder)
    {
        var rec = Symbol.IsRecord ? "record " : string.Empty;
        var kind = rec + GetTypeKind();
        var name = GetTypeName();

        builder.AppendLine($"partial {kind} {name}");
        builder.AppendLine("{");
        builder.IndentLevel++;

        OnPrint(context, builder);
        var num = false;

        foreach (var node in FieldChildren)
        {
            if (num) builder.AppendLine(); num = true;
            node.Print(context, builder);
        }
        foreach (var node in PropertyChildren)
        {
            if (num) builder.AppendLine(); num = true;
            node.Print(context, builder);
        }
        foreach (var node in MethodChildren)
        {
            if (num) builder.AppendLine(); num = true;
            node.Print(context, builder);
        }
        foreach (var node in TypeChildren)
        {
            if (num) builder.AppendLine(); num = true;
            node.Print(context, builder);
        }

        builder.IndentLevel--;
        builder.AppendLine("}");
    }

    /// <summary>
    /// Invoked to print the contents of this node.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="builder"></param>
    protected virtual void OnPrint(SourceProductionContext context, CodeBuilder builder) { }

    /// <summary>
    /// Invoked to obtain the name of this type. Inheritors can add to the returned name any
    /// inheritance chain they may need.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetTypeName() => Symbol.GivenName(addNullable: false);

    /// <summary>
    /// Invoked to get the kind of the given type.
    /// </summary>
    string GetTypeKind() => Symbol.TypeKind switch
    {
        TypeKind.Class => "class",
        TypeKind.Struct => "struct",
        TypeKind.Interface => "interface",

        _ => throw new ArgumentException("Invalid type kind.").WithData(Symbol, nameof(Symbol))
    };
}
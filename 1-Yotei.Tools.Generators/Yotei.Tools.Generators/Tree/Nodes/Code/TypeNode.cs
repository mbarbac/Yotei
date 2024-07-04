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
    public virtual bool Validate(SourceProductionContext context) => true;

    // -----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var head = GetHeader(context);
        if (head == null) return;

        cb.AppendLine(head);
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            EmitElements(context, cb);
            EmitCore(context, cb);
        }
        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to obtain the header of this type that, by default, it is just its kind followed
    /// by its name. Derived classes may add a colon (':') and an optional list of elements that
    /// the type inherits from or implements.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual string? GetHeader(SourceProductionContext context)
    {
        var rec = Symbol.IsRecord ? "record " : string.Empty;
        var kind = Symbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new ArgumentException("Invalid type kind.").WithData(Symbol)
        };

        /*
        var options = new EasyNameOptions(useGenerics: true);
        var name = Symbol.EasyName(options);
        return $"partial {rec}{kind} {name}";*/

        return $"partial {rec}{kind} {Symbol.Name}";
    }

    /// <summary>
    /// Invoked to emit the source code of the registered child elements, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitElements(SourceProductionContext context, CodeBuilder cb)
    {
        var done = false;

        foreach (var node in ChildFields)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildProperties)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildMethods)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildTypes)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
    }

    /// <summary>
    /// Gets the total number of child elements registered into this instance.
    /// </summary>
    protected int SumChilds
        => ChildFields.Count + ChildProperties.Count + ChildMethods.Count + ChildTypes.Count;

    /// <summary>
    /// Invoked to emit the source code of this type, without taking into consideration its
    /// registered child elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitCore(SourceProductionContext context, CodeBuilder cb) { }
}
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
    public virtual bool Validate(SourceProductionContext context)
    {
        if (!Symbol.IsPartial())
        {
            TreeDiagnostics.TypeIsNotPartial(Symbol).Report(context);
            return false;
        }

        if (!IsSupportedKind())
        {
            TreeDiagnostics.KindNotSupported(Symbol).Report(context);
            return false;
        }

        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) return false;

        return true;
    }

    /// <summary>
    /// Used to validate that the type's kind is supported for code generation.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsSupportedKind() => Symbol.TypeKind is
        TypeKind.Class or
        TypeKind.Struct or
        TypeKind.Interface;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var head = GetHeader(context);
        if (head == null) return;

        cb.AppendLine(head);
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            EmitChildElements(context, cb);
            EmitCore(context, cb);
        }
        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to emit the source code of this type node, without taking into consideration the
    /// source code of its registered child elements.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitCore(SourceProductionContext context, CodeBuilder cb) { }

    /// <summary>
    /// Invoked to obtain the header of this type that, by default, it is just its kind followed
    /// by its name (as in 'class MyType'). Derived classes can override this method to addd a
    /// colon (':') and an optional list of elements that the type inherits from or implements.
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

        var options = RoslynNameOptions.Default;
        var name = Symbol.EasyName(options);
        return $"partial {rec}{kind} {name}";
    }

    /// <summary>
    /// Invoked to emit the actual source code of the child elements registered in this type node.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitChildElements(SourceProductionContext context, CodeBuilder cb)
    {
        var done = false;

        foreach (var node in ChildTypes)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildProperties)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildFields)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildMethods)
        {
            if (done) cb.AppendLine(); done = true;
            node.Emit(context, cb);
        }
    }
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type node in the source code generation hierarchy.
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

    /// <inheritdoc/>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <inheritdoc/>
    public INode ParentNode { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The collection of child types registered in this instance.
    /// </summary>
    public ChildList<TypeNode> ChildTypes { get; } = [];

    /// <summary>
    /// The collection of child properties registered in this instance.
    /// </summary>
    public ChildList<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields registered in this instance.
    /// </summary>
    public ChildList<FieldNode> ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods registered in this instance.
    /// </summary>
    public ChildList<MethodNode> ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context)
    {
        if (!OnValidate(context)) return false;

        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) return false;
        return true;
    }

    /// <summary>
    /// Invoked to validate just this node.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        if (!context.TypeIsPartial(Symbol)) return false;
        if (!context.TypeKindIsSupported(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var rec = Symbol.IsRecord ? "record " : string.Empty;
        var kind = rec + GetKind();
        var header = GetTypeHeader();

        cb.AppendLine($"partial {kind} {header}");
        cb.AppendLine("{");
        cb.IndentLevel++;

        OnEmit(context, cb);
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

        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to emit the source code of just this node.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void OnEmit(SourceProductionContext context, CodeBuilder cb) { }

    /// <summary>
    /// Gets the string that represents the symbol kind.
    /// </summary>
    string GetKind() => Symbol.TypeKind switch
    {
        TypeKind.Class => "class",
        TypeKind.Struct => "struct",
        TypeKind.Interface => "interface",

        _ => throw new ArgumentException("Invalid type kind.").WithData(Symbol)
    };

    /// <summary>
    /// Invoked when generating source code to obtain the header of the type, it being the type
    /// name followed by a colon and any inheritance chain that might be appropriate.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetTypeHeader()
    {
        var options = new EasyNameOptions(useGenerics: true);
        return Symbol.EasyName(options);
    }
}
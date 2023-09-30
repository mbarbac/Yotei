namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal partial class TypeNode : Node
{
    /// <summary>
    /// Initializes a new instance for the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public TypeNode(Node parent, TypeCandidate candidate) : base(
        parent!.Generator ??
        throw new ArgumentNullException(nameof(parent)))
    {
        Parent = ValidateParent(parent);
        Candidate = candidate.ThrowWhenNull(nameof(candidate));
        Symbol = candidate.Symbol;

        ChildTypes = new();
        ChildProperties = new();
        ChildFields = new();
        ChildMethods = new();
    }

    /// <summary>
    /// The candidate from which this instance was obtained, or null if it was not created in
    /// the hierarchy formation phase.
    /// </summary>
    public TypeCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance, using the given symbol.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(Node parent, ITypeSymbol symbol) : base(
        parent!.Generator ??
        throw new ArgumentNullException(nameof(parent)))
    {
        Parent = ValidateParent(parent);
        Symbol = symbol.ThrowWhenNull(nameof(symbol));

        ChildTypes = new();
        ChildProperties = new();
        ChildFields = new();
        ChildMethods = new();
    }

    static Node ValidateParent(Node parent)
    {
        if (parent is not NamespaceNode and not TypeNode)
            throw new ArgumentException(
                "Parent is not a namespace or type node.")
                .WithData(parent, nameof(parent));

        return parent;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// The node in the hierarchy that contains this instance.
    /// </summary>
    public Node Parent { get; }

    /// <summary>
    /// The symbol associated with this instance.
    /// </summary>
    public ITypeSymbol Symbol { get; }

    /// <summary>
    /// Determines if this instance represents an interface, or not.
    /// </summary>
    public bool IsInterface => Symbol.TypeKind == TypeKind.Interface;

    /// <summary>
    /// Determines if this instance represents a <c>record</c>, or not.
    /// </summary>
    public bool IsRecord => Symbol.IsRecord;

    /// <summary>
    /// Determines if this instance represents an <c>abstract</c> one, or not.
    /// </summary>
    public bool IsAbstract => Symbol.IsAbstract;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child types registered into this instance.
    /// </summary>
    public ChildTypes ChildTypes { get; }

    /// <summary>
    /// The collection of child properties registered into this instance.
    /// </summary>
    public ChildProperties ChildProperties { get; }

    /// <summary>
    /// The collection of child fields registered into this instance.
    /// </summary>
    public ChildFields ChildFields { get; }

    /// <summary>
    /// The collection of child methods registered into this instance.
    /// </summary>
    public ChildMethods ChildMethods { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (Candidate != null
            ? !ValidateIsPartial(context, Candidate)
            : !ValidateIsPartial(context, Symbol))
            return false;

        if (!ValidateIsSupported(context, Symbol)) return false;

        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> This method delegates the printing of the type specific code to the new 'OnPrint'
    /// method, which shall be the one overriden, instead of this one, except for very rare
    /// circumstances.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        var kind = GetKind();
        var name = GetTypeNameDecorated();

        // Header...
        cb.AppendLine($"partial {kind} {name}");
        cb.AppendLine("{");
        cb.IndentLevel++;

        var num = 0;

        // Child fields...
        foreach (var node in ChildFields)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }

        // Child properties...
        foreach (var node in ChildProperties)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }

        // Child methods...
        foreach (var node in ChildMethods)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }

        // Other type elements not yet covered...
        OnPrint(context, cb);

        // Child types...
        foreach (var node in ChildTypes)
        {
            if (num > 0) cb.AppendLine();
            num++;
            node.Print(context, cb);
        }

        // Finishing...
        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to emit the actual source of this type, after its child elements have been
    /// already printed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void OnPrint(SourceProductionContext context, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to get a string with the kind of this type.
    /// </summary>
    string GetKind()
    {
        var rec = Symbol.IsRecord ? "record " : string.Empty;

        return Symbol.TypeKind switch
        {
            TypeKind.Class => $"{rec}class",
            TypeKind.Struct => $"{rec}struct",
            TypeKind.Interface => "interface",

            _ => throw new UnExpectedException($"Invalid type kind for symbol: {Symbol}")
        };
    }

    /// <summary>
    /// Invoked to obtain the decorated type name of this instance, which shall be its name
    /// followed by any interfaces it needs to implement, as in: 'TA : IB, IC ...'.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetTypeNameDecorated() => Symbol.Name;
}
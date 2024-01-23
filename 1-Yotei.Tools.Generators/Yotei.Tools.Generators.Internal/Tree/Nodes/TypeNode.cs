namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : INode
{
    INode Initialize(INode parent)
    {
        parent.ThrowWhenNull();

        if (parent is not NamespaceNode and not TypeNode) throw new ArgumentException(
            "Parent node is not a namespace or a type one.")
            .WithData(parent);

        return parent;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(INode parent, INamedTypeSymbol symbol)
    {
        ParentNode = Initialize(parent);
        Symbol = symbol.ThrowWhenNull();

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
    {
        ParentNode = Initialize(parent);
        Candidate = candidate.ThrowWhenNull();
        Symbol = candidate.Symbol;

        TypeChildren = new(this);
        PropertyChildren = new(this);
        FieldChildren = new(this);
        MethodChildren = new(this);
    }

    // <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.EasyName()}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Hierarchy Hierarchy => ParentNode.Hierarchy;

    /// <summary>
    /// The parent node of this instance, it being either a namespace or a parent type one.
    /// </summary>
    public INode ParentNode { get; }

    /// <summary>
    /// The symbol of the associated type.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The candidate of this instance, or null if not created through a generator.
    /// </summary>
    public TypeCandidate? Candidate { get; }

    /// <summary>
    /// The collection of type nodes registered into this instance.
    /// </summary>
    public TypeChildrenList TypeChildren { get; }
    public class TypeChildrenList : ChildrenList<TypeNode>
    {
        public TypeChildrenList(TypeNode master) : base(master)
        {
            ItemToDebug = (item) => item.Symbol.Name;
            Comparer = (x, y) => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
        }
    }

    /// <summary>
    /// The collection of property nodes registered into this instance.
    /// </summary>
    public PropertyChildrenList PropertyChildren { get; }
    public class PropertyChildrenList : ChildrenList<PropertyNode>
    {
        public PropertyChildrenList(TypeNode master) : base(master)
        {
            ItemToDebug = (item) => item.Symbol.Name;
            Comparer = (x, y) => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
        }
    }

    /// <summary>
    /// The collection of field nodes registered into this instance.
    /// </summary>
    public FieldChildrenList FieldChildren { get; }
    public class FieldChildrenList : ChildrenList<FieldNode>
    {
        public FieldChildrenList(TypeNode master) : base(master)
        {
            ItemToDebug = (item) => item.Symbol.Name;
            Comparer = (x, y) => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
        }
    }

    /// <summary>
    /// The collection of method nodes registered into this instance.
    /// </summary>
    public MethodChildrenList MethodChildren { get; }
    public class MethodChildrenList : ChildrenList<MethodNode>
    {
        public MethodChildrenList(TypeNode master) : base(master)
        {
            ItemToDebug = (item) => item.Symbol.Name;
            Comparer = (x, y) => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context)
    {
        foreach (var node in TypeChildren) if (!node.Validate(context)) return false;
        foreach (var node in PropertyChildren) if (!node.Validate(context)) return false;
        foreach (var node in FieldChildren) if (!node.Validate(context)) return false;
        foreach (var node in MethodChildren) if (!node.Validate(context)) return false;

        return true;
    }

    /// <summary>
    /// Invoked to validate this node only.
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Print(SourceProductionContext context, CodeBuilder cb)
    {
        var rec = Symbol.IsRecord ? "record " : string.Empty;
        var kind = rec + GetTypeKind();
        var name = GetTypeName();

        cb.AppendLine($"partial {kind} {name}");
        cb.AppendLine("{");
        cb.IndentLevel++;

        OnPrint(context, cb);
        var num = false;

        foreach (var node in FieldChildren)
        {
            if (num) cb.AppendLine();
            num = true;
            node.Print(context, cb);
        }

        foreach (var node in PropertyChildren)
        {
            if (num) cb.AppendLine();
            num = true;
            node.Print(context, cb);
        }

        foreach (var node in MethodChildren)
        {
            if (num) cb.AppendLine();
            num = true;
            node.Print(context, cb);
        }

        foreach (var node in TypeChildren)
        {
            if (num) cb.AppendLine();
            num = true;
            node.Print(context, cb);
        }

        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Invoked to emit the customized contents of this node.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void OnPrint(SourceProductionContext context, CodeBuilder cb) { }

    /// <summary>
    /// Invoked to obtain the type name that will be emitted.
    /// <br/> Inheritors may add to it any inheritance chain that would be needed.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetTypeName()
    {
        var options = EasyNameOptions.Default with
        {
            UseFullTypeName = false,
            UseTypeParameters = true,
            UseNullableAnnotation = false,
        };
        return Symbol.EasyName(options);
    }

    /// <summary>
    /// Obtains the kind of this type.
    /// </summary>
    /// <returns></returns>
    string GetTypeKind() => Symbol.TypeKind switch
    {
        TypeKind.Class => "class",
        TypeKind.Struct => "struct",
        TypeKind.Interface => "interface",

        _ => throw new ArgumentException("Invalid type kind.").WithData(Symbol)
    };
}
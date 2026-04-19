namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a type-alike source code generation node.
/// </summary>
public partial class TypeNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public TypeNode(INamedTypeSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.SyntaxNodes"/>
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];
    List<SyntaxNode> ITreeNode.SyntaxNodes => (List<SyntaxNode>)SyntaxNodes.Cast<SyntaxNode>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance was created with the sole purpose of holding child elements,
    /// or rather because it was identified as a source code generation node by itself.
    /// </summary>
    public bool ChildsOnly { get; init; }

    /// <summary>
    /// The collection of child properties known to this instance.
    /// </summary>
    public List<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields known to this instance.
    /// </summary>
    public List<FieldNode> ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods known to this instance.
    /// </summary>
    public List<MethodNode> ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not TypeNode valid) return false;

        if (!SymbolEqualityComparer.Default.Equals(Symbol, valid.Symbol)) return false;
        if (!Attributes.SequenceEqual(valid.Attributes)) return false;

        if (!ChildProperties.SequenceEqual(valid.ChildProperties, new MyComparer<PropertyNode>())) return false;
        if (!ChildFields.SequenceEqual(valid.ChildFields, new MyComparer<FieldNode>())) return false;
        if (!ChildMethods.SequenceEqual(valid.ChildMethods, new MyComparer<MethodNode>())) return false;
        return true;
    }
    struct MyComparer<T> : IEqualityComparer<T> where T : ITreeNode
    {
        readonly static SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
        public readonly bool Equals(T x, T y) => Comparer.Equals(x.Symbol, y.Symbol);
        public readonly int GetHashCode(T obj) => Comparer.GetHashCode(obj.Symbol);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int code = SymbolEqualityComparer.Default.GetHashCode(Symbol);
        foreach (var attr in Attributes) code = HashCode.Combine(code, attr);
        foreach (var node in ChildProperties) code = HashCode.Combine(code, node);
        foreach (var node in ChildFields) code = HashCode.Combine(code, node);
        foreach (var node in ChildMethods) code = HashCode.Combine(code, node);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    public virtual void Augment(TypeNode other)
    {
        foreach (var syntax in other.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in other.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);

        var comparer = SymbolEqualityComparer.Default;

        foreach (var item in other.ChildProperties)
            if (ChildProperties.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildProperties.Add(item);

        foreach (var item in other.ChildFields)
            if (ChildFields.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildFields.Add(item);

        foreach (var item in other.ChildMethods)
            if (ChildMethods.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildMethods.Add(item);
    }
    void ITreeNode.Augment(ITreeNode other) => Augment((TypeNode)other);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the base list (what follows the semicolon character) that shall be added
    /// to the type header, or null if any.
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetBaseList() => null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this instance before emitting its source code.
    /// <br/> If this method returns <see langword="false"/>, then its code generation is aborted.
    /// <br/> Inheritors will typically invoke their base method first.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        var r = true;

        if (!Symbol.IsPartial) { TreeError.TypeNotPartial.Report(Symbol, context); r = false; }
        if (!IsSupportedKind()) { TreeError.KindNotSupported.Report(Symbol, context); r = false; }
        return r;
    }

    /// <summary>
    /// Determines if this type is of a supported kind, or not.
    /// <br/> Inheritors will typically invoke their base method first.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsSupportedKind() => Symbol.TypeKind is
        TypeKind.Class or
        TypeKind.Struct or
        TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the actual source code generated for this type element, without taking
    /// into consideration its headers or its child ones.
    /// <br/> If this method returns <see langword="false"/>, then its code generation is aborted.
    /// <br/> Inheritors will typically completely override their base method.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitCore(ref TreeContext context, CodeBuilder cb) => true;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the child elements captured at this instance.
    /// <br/> If this method returns <see langword="false"/>, then its code generation is aborted.
    /// <br/> Inheritors will typically invoke their base method first.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitChilds(ref TreeContext context, CodeBuilder cb)
    {
        var r = true;
        var n = false;

        foreach (var node in ChildProperties)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(ref context, cb)) r = false;
        }

        foreach (var node in ChildFields)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(ref context, cb)) r = false;
        }

        foreach (var node in ChildMethods)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(ref context, cb)) r = false;
        }

        return r;
    }
}
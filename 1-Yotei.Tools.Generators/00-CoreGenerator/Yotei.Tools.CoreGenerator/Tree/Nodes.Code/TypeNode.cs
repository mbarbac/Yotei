namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike source code generation element.
/// </summary>
internal partial class TypeNode : ITreeNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public TypeNode(INamedTypeSymbol symbol) => Symbol = symbol.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    INode ITreeNode.ParentNode => null!;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; }
    ISymbol ITreeNode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ITreeNode.SyntaxNodes"/>
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];
    List<SyntaxNode> ITreeNode.SyntaxNodes
        => (List<SyntaxNode>)SyntaxNodes.Cast<BaseTypeDeclarationSyntax>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance was created for the sole purpose of holding child elements,
    /// and not because it was identified as a source code generation element by itself.
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

    /// <summary>
    /// The collection of child events known to this instance.
    /// </summary>
    public List<EventNode> ChildEvents { get; } = [];
    
    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void Augment(TypeNode node)
    {
        foreach (var syntax in node.SyntaxNodes)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(syntax)) == null)
                SyntaxNodes.Add(syntax);

        foreach (var at in node.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);

        var comparer = SymbolEqualityComparer.Default;

        foreach (var item in node.ChildProperties)
            if (ChildProperties.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildProperties.Add(item);

        foreach (var item in node.ChildFields)
            if (ChildFields.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildFields.Add(item);

        foreach (var item in node.ChildMethods)
            if (ChildMethods.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildMethods.Add(item);

        foreach (var item in node.ChildEvents)
            if (ChildEvents.Find(x => comparer.Equals(x.Symbol, item.Symbol)) == null)
                ChildEvents.Add(item);
    }
    void ITreeNode.Augment(ITreeNode node) => Augment((TypeNode)node);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this node at source code generation time. If this method returns
    /// <see langword="false"/>, source code generation is aborted. Derived types must invoke
    /// their base overriden methods first.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        if (!Symbol.IsPartial) { Symbol.ReportError(TreeError.TypeNotPartial, context); return false; }
        if (!IsSupportedKind()) { Symbol.ReportError(TreeError.KindNotSupported, context); return false; }

        return true;
    }

    /// <summary>
    /// Determines if this type is of a supported kind, or not.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsSupportedKind() => Symbol.TypeKind is
        TypeKind.Class or
        TypeKind.Struct or
        TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain a string with the  base list that shall follow the type's name (as in
    /// 'Name : TBase, IFace1, ...'), or <see langword="null"/> if not needed (which is what is
    /// returned by default). Derived types shall override this method as needed.
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetBaseList() => null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of this type per-se -  so not not dealing with the type's
    /// headers, base list, or with any child element this node may carry. By default this method
    /// does nothing. Derived types can override this method as needed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitCore(SourceProductionContext context, CodeBuilder cb) => true;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to invoke the emission of the source code of the child elements carried by this
    /// instance, if any. If derived types override this method, then they either take complete
    /// control of this process, or invoke their base methods.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitChilds(SourceProductionContext context, CodeBuilder cb)
    {
        var r = true;
        var n = false;

        foreach (var node in ChildProperties)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        foreach (var node in ChildFields)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        foreach (var node in ChildMethods)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        foreach (var node in ChildEvents)
        {
            if (n) cb.AppendLine(); n = true;
            if (!node.Emit(context, cb)) r = false;
        }

        return r;
    }
}
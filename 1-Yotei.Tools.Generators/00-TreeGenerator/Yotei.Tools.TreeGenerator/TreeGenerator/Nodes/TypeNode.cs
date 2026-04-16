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
    /// or rather it was identified as a source code generation node by itself.
    /// </summary>
    public bool ChildsOnly { get; init; }

    /// <summary>
    /// The collection of child properties known to this instance.
    /// </summary>
    public List<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The collection of child fields known to this instance.
    /// </summary>
    public List<PropertyNode> ChildFields { get; } = [];

    /// <summary>
    /// The collection of child methods known to this instance.
    /// </summary>
    public List<PropertyNode> ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(INode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not TypeNode valid) return false;

        return SymbolEqualityComparer.Default.Equals(Symbol, valid.Symbol);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(Symbol);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ITreeNode.Augment(ITreeNode)"/>
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
    /// <inheritdoc/> Inheritors can customize its behavior by overriding the
    /// <see cref="GetBaseList()"/>, <see cref="OnValidate(SourceProductionContext)"/>,
    /// <see cref="IsSupportedKind()"/>,
    /// <see cref="OnEmitCore(SourceProductionContext, CodeBuilder)"/> and
    /// <see cref="OnEmitChilds(SourceProductionContext, CodeBuilder)"/> methods.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    public bool Emit(SourceProductionContext context, CodeBuilder cb)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        // Parent elements, returning how many levels were opened...
        var num = OnEmitParents(context, cb);

        // This type...
        var head = GetTypeHeader(Symbol);
        var str = GetBaseList();
        if ((str = str.NullWhenEmpty(trim: true)) != null) head += $" : {str}";

        cb.AppendLine(head);
        cb.AppendLine("{");
        cb.IndentLevel++;

        var old = cb.Length;
        var ret = OnEmitCore(context, cb);
        if (ret)
        {
            var len = cb.Length;
            if (len != old) cb.AppendLine();

            if (!OnEmitChilds(context, cb)) ret = false;
        }
        cb.IndentLevel--;
        cb.AppendLine("}");

        // Closing he parent levels...
        for (var i = 0; i < num; i++)
        {
            cb.IndentLevel--;
            cb.AppendLine("}");
        }

        // Returning...
        return ret;
    }

    /// <summary>
    /// Invoked to obtain the base list (as in 'header : TBase, IFace1, ...') that shall follow
    /// the type's header, or null if it is not needed. If not null, the returned base list must
    /// not include the ':' character.
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetBaseList() => null;

    /// <summary>
    /// Invoked to validate this instance before emitting its source code. If this method returns
    /// false, then the source code generation is aborted. Inheritors typically will invoke their
    /// base methods first.
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
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsSupportedKind() => Symbol.TypeKind is
        TypeKind.Class or
        TypeKind.Struct or
        TypeKind.Interface;

    /// <summary>
    /// Invoked to emit the actual source code generated for this type element, without taking
    /// into consideration its headers or its child elements.
    /// <br/> If this method returns <see langword="false"/>, then its source code generation
    /// is aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    protected virtual bool OnEmitCore(SourceProductionContext context, CodeBuilder cb) => true;

    /// <summary>
    /// Invoked to emit the source code of the child elements captured by this instance. If this
    /// method returns <see langword="false"/>, then its source code generation is aborted.
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

        return r;
    }
}
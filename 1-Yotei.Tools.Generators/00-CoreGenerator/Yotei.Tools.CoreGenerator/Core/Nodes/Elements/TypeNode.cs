namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : INode
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

    /// <summary>
    /// Obtains the file name where the code of this type, and of its child elements, shall be
    /// emitted.
    /// </summary>
    /// <returns></returns>
    public virtual string GetFileName() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance was created just to hold its child elements. If not, it means
    /// the associated type was identified by its own (even if this instance contains other child
    /// elements).
    /// </summary>
    public bool IsHolderOnly { get; init; }

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public INamedTypeSymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured for this instance, or an empty one if any.
    /// </summary>
    public List<BaseTypeDeclarationSyntax> SyntaxNodes { get; } = [];

    /// <summary>
    /// The collection of attributes by which this candidate was identified
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];

    // ----------------------------------------------------

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
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// candidate. This method is invoked by the hierarchy-creation process when a node for the
    /// element already exist in that hierarchy.
    /// </summary>
    /// <param name="candidate"></param>
    public virtual void Augment(TypeCandidate candidate)
    {
        if (candidate.Syntax is not null)
            if (SyntaxNodes.Find(x => x.IsEquivalentTo(candidate.Syntax)) == null)
                SyntaxNodes.Add(candidate.Syntax);

        foreach (var at in candidate.Attributes)
            if (Attributes.Find(x => x.EqualsTo(at)) == null)
                Attributes.Add(at);
    }

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// node. This method is invoked by the hierarchy-creation process when a node for the element
    /// already exist in that hierarchy.
    /// </summary>
    /// <param name="node"></param>
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
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context)
    {
        if (!Symbol.IsPartial) { Symbol.ReportError(CoreError.TypeNotPartial, context); return false; }
        if (!IsSupportedKind()) { Symbol.ReportError(CoreError.KindNotSupported, context); return false; }

        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) return false;

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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain the appropriate header for the type of this instance, that typically is
    /// 'partial {typename}'. Derived types can override this method to add a base list to it, as
    /// in '... : TBase, IFace, ...'.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetHeader() => BuildHeader(Symbol);

    /// <summary>
    /// Invoked to obtain the appropriate header for the given type, as in 'partial {typename}'.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string BuildHeader(INamedTypeSymbol symbol)
    {
        var rec = symbol.IsRecord ? "record " : string.Empty;
        string kind = symbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new ArgumentException("Type kind not supported.").WithData(symbol.Name)
        };

        var options = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        var name = symbol.ToDisplayString(options);
        return $"partial {rec}{kind} {name}";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of this type, only (ie: not the source code of its child
    /// elements).
    /// </summary>
    protected virtual void EmitCore(SourceProductionContext context, CodeBuilder cb) { }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of the child elements of this type.
    /// </summary>
    protected virtual void EmitChilds(SourceProductionContext context, CodeBuilder cb)
    {
        var nl = false;

        foreach (var node in ChildFields)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildProperties)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
        foreach (var node in ChildMethods)
        {
            if (nl) cb.AppendLine(); nl = true;
            node.Emit(context, cb);
        }
    }
}
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
    public TypeNode(INamedTypeSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="INode.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol INode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="INode.Syntax"/>
    /// </summary>
    public TypeDeclarationSyntax? Syntax { get; init; }
    SyntaxNode? INode.Syntax => Syntax;

    /// <summary>
    /// The attributes captured for this instance, or '<c>empty</c>' if any, or if this data is
    /// not available.
    /// </summary>
    public ImmutableArray<AttributeData> Attributes
    {
        get;
        init => field = value.Length == 0 ? [] : (value.Any(x => x is null)
            ? throw new ArgumentException("Collection of attributes carries null elements.").WithData(value)
            : value);
    } = [];

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child properties registered into this instance.
    /// </summary>
    public CustomList<PropertyNode> ChildProperties { get; }
    = new() { AreEqual = (x, y) => Compare(x.Symbol, y.Symbol, strict: true) };

    /// <summary>
    /// Determines if the two given properties can be considered the same, or not. In strict mode,
    /// the types of the indexes of the indexed properties must be the same. If not, it is enough
    /// if the right ones can be assigned to the left ones. If the property is not an indexed one,
    /// the '<paramref name="strict"/>' argument is ignored.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public static bool Compare(IPropertySymbol x, IPropertySymbol y, bool strict)
    {
        if (string.Compare(x.Name, y.Name) != 0) return false;
        if (x.Parameters.Length != y.Parameters.Length) return false;

        for (int i = 0; i < x.Parameters.Length; i++)
        {
            var xpar = x.Parameters[i];
            var ypar = y.Parameters[i];
            var same = strict
                ? SymbolEqualityComparer.Default.Equals(xpar, ypar)
                : ypar.Type.IsAssignableTo(xpar.Type);

            if (!same) return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child fields registered into this instance.
    /// </summary>
    public CustomList<FieldNode> ChildFields { get; }
    = new() { AreEqual = (x, y) => Compare(x.Symbol, y.Symbol) };

    /// <summary>
    /// Determines if the two given methods can be considered the same, or not. In strict mode,
    /// the types of the respective arguments must be the same. If not, it is enough if the right
    /// ones can be assigned to the left ones.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public static bool Compare(
        IFieldSymbol x, IFieldSymbol y) => string.Compare(x.Name, y.Name) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child methods registered into this instance.
    /// </summary>
    public CustomList<MethodNode> ChildMethods { get; }
    = new() { AreEqual = (x, y) => Compare(x.Symbol, y.Symbol, strict: true) };

    /// <summary>
    /// Determines if the two given methods can be considered the same, or not. In strict mode,
    /// the types of the respective arguments must be the same. If not, it is enough if the right
    /// ones can be assigned to the left ones.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public static bool Compare(IMethodSymbol x, IMethodSymbol y, bool strict)
    {
        if (string.Compare(x.Name, y.Name) != 0) return false;
        if (x.Parameters.Length != y.Parameters.Length) return false;

        for (int i = 0; i < x.Parameters.Length; i++)
        {
            var xpar = x.Parameters[i];
            var ypar = y.Parameters[i];
            var same = strict
                ? SymbolEqualityComparer.Default.Equals(xpar, ypar)
                : ypar.Type.IsAssignableTo(xpar.Type);

            if (!same) return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (!Symbol.IsPartial) { CoreDiagnostics.TypeNotPartial(Symbol).Report(context); r = false; }
        if (!IsSupportedKind()) { CoreDiagnostics.KindNotSupported(Symbol).Report(context); r = false; }

        foreach (var node in ChildProperties) if (!node.Validate(context)) r = false;
        foreach (var node in ChildFields) if (!node.Validate(context)) r = false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) r = false;

        return r;
    }

    /// <summary>
    /// Determines if the type's kind is supported for code generation.
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
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        var head = GetHeader(context);
        if (head == null) return;

        cb.AppendLine(head);
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            var old = cb.Length; EmitChilds(context, cb);
            var len = cb.Length;
            EmitCore(context, cb, old != len);
        }
        cb.IndentLevel--;
        cb.AppendLine("}");
    }

    /// <summary>
    /// Returns the header of the emitted type, which by default is of the form 'partial Name'.
    /// Inheritors can add a base list, such as ': TBase, IFace...', as needed.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual string GetHeader(SourceProductionContext context)
    {
        var rec = Symbol.IsRecord ? "record " : string.Empty;

        string kind;
        switch (Symbol.TypeKind)
        {
            case TypeKind.Class: kind = "class"; break;
            case TypeKind.Struct: kind = "struct"; break;
            case TypeKind.Interface: kind = "interface"; break;
            default:
                CoreDiagnostics.KindNotSupported(Symbol).Report(context);
                throw new ArgumentException("Invalid type kind.").WithData(Symbol);
        }

        var options = EasyNameOptions.Default;
        var name = Symbol.EasyName(options);
        return $"partial {rec}{kind} {name}";
    }

    /// <summary>
    /// Invoked to emit the source code of this type, without taking into consideration its child
    /// elements. If '<paramref name="needNL"/>' is '<c>true</c>', then it is expected that this
    /// method emits a new line before appending its own contents.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitCore(SourceProductionContext context, CodeBuilder cb, bool needNL)
    { }

    /// <summary>
    /// Invoked to emit the source code of the child elements of this instance, if any, without
    /// taking into consideration the type itself.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void EmitChilds(SourceProductionContext context, CodeBuilder cb)
    {
        var done = false;

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
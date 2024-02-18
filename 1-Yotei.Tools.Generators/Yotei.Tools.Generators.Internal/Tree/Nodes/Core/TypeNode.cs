using System.Data;

namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type in the source code generation hierarchy.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class TypeNode(
    SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    : Candidate(model, syntax, symbol), INode, ITypeCandidate
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var options = new EasyNameOptions(useGenerics: true);
        return $"Type: {Symbol.EasyName(options)}";
    }

    /// <inheritdoc/>
    public new TypeDeclarationSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public new INamedTypeSymbol Symbol { get; } = symbol.ThrowWhenNull();

    /// <summary>
    /// The list of child types.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<TypeNode> ChildTypes { get; } = [];

    /// <summary>
    /// The list of child properties.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<PropertyNode> ChildProperties { get; } = [];

    /// <summary>
    /// The list of child fields.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<FieldNode> ChildFields { get; } = [];

    /// <summary>
    /// The list of child methods.
    /// <br/> Default equality: symbol comparison.
    /// </summary>
    public List<MethodNode> ChildMethods { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context)
    {
        OnValidate(context);

        foreach (var node in ChildTypes) if (!node.Validate(context)) return false;
        foreach (var node in ChildProperties) if (!node.Validate(context)) return false;
        foreach (var node in ChildFields) if (!node.Validate(context)) return false;
        foreach (var node in ChildMethods) if (!node.Validate(context)) return false;
        return true;
    }

    /// <summary>
    /// Invoked to validate the characteristics of this node.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual bool OnValidate(SourceProductionContext context)
    {
        if (!context.TypeIsPartial(Syntax)) return false;
        if (!context.TypeKindIsSupported(Syntax)) return false;

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
    /// Invoked when generating source code to obtain the header of the type, it being its name
    /// followed by any inheritance chain that might be appropriate.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetTypeHeader()
    {
        var options = new EasyNameOptions(useGenerics: true);
        return Symbol.EasyName(options);
    }

    /// <summary>
    /// Invoked to emit the source code of this type, not its hierarchical childs.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected virtual void OnEmit(SourceProductionContext context, CodeBuilder cb) { }
}
namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc/>
internal class XFieldNode(
    SemanticModel model, FieldDeclarationSyntax syntax, IFieldSymbol symbol)
    : FieldNode(model, syntax, symbol)
{
    XField XMember => _XMember ??= new XField(Symbol);
    XField? _XMember;

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;
        if (!XMember.Validate(context)) return false;

        return true;
    }

    /// <inheritdoc/>
    public override void Emit(
        SourceProductionContext context, CodeBuilder cb) => XMember.Emit(context, cb);
}

// ========================================================
class XField(IFieldSymbol symbol)
{
    public IFieldSymbol Symbol { get; } = symbol.ThrowWhenNull();
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// Validates this member.
    /// </summary>
    public bool Validate(SourceProductionContext context)
    {
        if (!Symbol.ContainingType.IsInterface() &&
            !context.FieldIsWrittable(Symbol)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the source code for this element.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb)
    {
    }
}
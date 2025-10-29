namespace Yotei.Tools.WithGenerator;

// ========================================================
internal partial class WithGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Type[] FieldAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override FieldNode CreateNode(
        TypeNode parent, FieldCandidate candidate)
        => new XFieldNode(parent, candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };
}

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XFieldNode : FieldNode, IXNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol)
    {
        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"v_{Symbol.Name}";
    }
    readonly string ArgumentName;

    public string MethodName { get; }
    public bool IsInherited { get; init; }
    public INamedTypeSymbol ReturnType { get; set; } = default!;
    public bool ReturnNullable { get; set; }
    public EasyNameOptions ReturnOptions { get; set; } = default!;
    public bool UseVirtual { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (Host.IsRecord) { CoreDiagnostics.RecordsNotSupported(Host).Report(context); r = false; }
        if (!Symbol.IsWrittable) { CoreDiagnostics.NotWrittable(Symbol).Report(context); r = false; }
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        // Explicitly declared or implemented...
        if (Host.FindMethod(true, MethodName, Symbol.Type, out _)) return;

        // Capturing working data...
        if (!this.CaptureEmit(context)) return;

        // Dispatching...
        if (Host.IsInterface) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext _, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var ptype = Symbol.Type.EasyName(EasyNameOptions.Full);
        var modifiers = this.GetInterfaceModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({ptype} {ArgumentName});");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext _, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var ptype = Symbol.Type.EasyName(EasyNameOptions.Full);
        var modifiers = this.GetAbstractModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({ptype} {ArgumentName});");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Host.FindCopyConstructor(strict: false);
        if (ctor == null) { CoreDiagnostics.NoCopyConstructor(Host).Report(context); return; }

        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var ptype = Symbol.Type.EasyName(EasyNameOptions.Full);
        var modifiers = this.GetRegularModifiers();
        var hostname = Host.EasyName();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({ptype} {ArgumentName})");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_host = new {hostname}(this)");
            cb.AppendLine("{");
            cb.IndentLevel++;
            {
                cb.AppendLine($"{Symbol.Name} = {ArgumentName}");
            }
            cb.IndentLevel--;
            cb.AppendLine("};");
            cb.AppendLine("return v_host;");
        }
        cb.IndentLevel--;
        cb.AppendLine("}");
    }
}
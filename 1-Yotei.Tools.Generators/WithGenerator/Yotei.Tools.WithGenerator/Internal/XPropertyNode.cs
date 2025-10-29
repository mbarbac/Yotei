namespace Yotei.Tools.WithGenerator;

// ========================================================
internal partial class WithGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Type[] PropertyAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override PropertyNode CreateNode(
        TypeNode parent, PropertyCandidate candidate)
        => new XPropertyNode(parent, candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };
}

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode, IXNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol)
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
        if (Symbol.IsIndexer) { CoreDiagnostics.IndexerNotSupported(Symbol).Report(context); r = false; }
        if (!Symbol.HasGetter) { CoreDiagnostics.NoGetter(Symbol).Report(context); r = false; }
        if (!Host.IsInterface && !Symbol.HasSetter) { CoreDiagnostics.NoSetter(Symbol).Report(context); r = false; }
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
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var ptype = Symbol.Type.EasyName(EasyNameOptions.Full);
        var modifiers = this.GetAbstractModifiers();

        XNode.EmitDocumentation(Symbol, cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull}");
        cb.AppendLine($"{MethodName}({ptype} {ArgumentName});");

        EmitExplicitInterfaces(context, cb);
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

        EmitExplicitInterfaces(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the interfaces that need explicit implementation.
    /// </summary>
    void EmitExplicitInterfaces(SourceProductionContext _, CodeBuilder cb)
    {
        var ifaces = GetExplicitInterfaces();
        foreach (var iface in ifaces)
        {
            var type = Symbol.Type;
            var found = iface.FindDecoratedMember(true, Symbol.Name, out var member, iface.AllInterfaces);
            if (found) type = ((IPropertySymbol)member!).Type;
            else
            {
                found = iface.FindMethod(true, Symbol.Name, (INamedTypeSymbol)Symbol.Type, out var method, iface.AllInterfaces);
                if (found) type = method!.Parameters[0].Type;
            }
            var argtype = type.EasyName(EasyNameOptions.Full);
            var nullable = ReturnNullable ? "?" : string.Empty;
            var typename = iface.EasyName(EasyNameOptions.Full with { TypeUseNullable = false });

            cb.AppendLine();
            cb.AppendLine($"{typename}{nullable}");
            cb.Append($"{typename}.{MethodName}({argtype} value)");
            cb.AppendLine($" => {MethodName}(value);");
            continue;
        }
    }

    /// <summary>
    /// Gets the collection of interfaces that need explicit implementation.
    /// </summary>
    List<INamedTypeSymbol> GetExplicitInterfaces()
    {
        List<INamedTypeSymbol> list = [];
        foreach (var iface in Host.AllInterfaces) TryCapture(iface);
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        void TryCapture(INamedTypeSymbol iface)
        {
            var comparer = SymbolEqualityComparer.Default;

            if (iface.FindDecoratedMember(true, Symbol.Name, out _) ||
                iface.FindMethod(true, Symbol.Name, (INamedTypeSymbol)Symbol.Type, out _))
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp is null) list.Add(iface);
            }
        }
    }
}
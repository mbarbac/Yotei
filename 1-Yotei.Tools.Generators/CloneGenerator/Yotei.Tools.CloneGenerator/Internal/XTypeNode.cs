namespace Yotei.Tools.CloneGenerator;

// ========================================================
internal partial class CloneGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Type[] TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TypeNode CreateNode(
        TypeCandidate candidate)
        => new XTypeNode(candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };
}

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        ReturnType = Symbol;
        ReturnNullable = false;
        ReturnOptions = EasyNameOptions.Default;
        UseVirtual = true;
    }
    internal INamedTypeSymbol ReturnType;
    internal bool ReturnNullable;
    internal EasyNameOptions ReturnOptions;
    internal bool UseVirtual;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Symbol.IsRecord) { CoreDiagnostics.RecordsNotSupported(Symbol).Report(context); r = false; }
        if (Attributes.Length == 0) { CoreDiagnostics.NoAttributes(Symbol).Report(context); r = false; }
        if (Attributes.Length > 1) { CoreDiagnostics.TooManyAttributes(Symbol).Report(context); r = false; }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures relevant working data at 'Emit' time.
    /// </summary>
    void CaptureEmit()
    {
        // Validate(...) ensures we have 1 and only 1 attribute...
        var at = Attributes[0];

        if (at.GetReturnType(out var type, out var nullable))
        {
            ReturnType = type;
            ReturnNullable = nullable;

            var same = SymbolEqualityComparer.Default.Equals(Symbol, type);
            if (!same) ReturnOptions = EasyNameOptions.Full with
            { TypeUseNullable = false };
        }

        if (at.GetUseVirtual(out var temp)) UseVirtual = temp;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        // Explicitly declared or implemented...
        if (Symbol.FindMethod(true, out _)) return;

        // Capturing working data...
        CaptureEmit();

        // Dispatching...
        if (Symbol.IsInterface) EmitHostInterface(context, cb);
        else if (Symbol.IsAbstract) EmitHostAbstract(context, cb);
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
        var modifiers = this.GetInterfaceModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var modifiers = this.GetAbstractModifiers();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone();");

        EmitExplicitInterfaces(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        var ctor = Symbol.FindCopyConstructor(strict: false);
        if (ctor == null) { CoreDiagnostics.NoCopyConstructor(Symbol).Report(context); return; }

        var rtype = ReturnType.EasyName(ReturnOptions);
        var rnull = ReturnNullable ? "?" : string.Empty;
        var modifiers = this.GetRegularModifiers(context);
        var hostname = Symbol.EasyName();

        EmitDocumentation(cb);
        cb.AppendLine($"{modifiers}{rtype}{rnull} Clone()");
        cb.AppendLine("{");
        cb.IndentLevel++;
        {
            cb.AppendLine($"var v_host = new {hostname}(this);");
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
        var options = EasyNameOptions.Full with { TypeUseNullable = false };
        var ifaces = GetExplicitInterfaces();
        foreach (var iface in ifaces)
        {
            var typename = iface.EasyName(options);
            var nullable = ReturnNullable ? "?" : string.Empty;
            var valuename = iface.Name == "ICloneable" ? "object" : $"{typename}{nullable}";

            cb.AppendLine();
            cb.AppendLine(valuename);
            cb.AppendLine($"{typename}.Clone()");
            cb.AppendLine($"=> ({valuename})Clone();");
        }
    }

    /// <summary>
    /// Gets the collection of interfaces that need explicit implementation.
    /// </summary>
    List<INamedTypeSymbol> GetExplicitInterfaces()
    {
        var comparer = SymbolEqualityComparer.Default;
        List<INamedTypeSymbol> list = [];
        foreach (var iface in Symbol.Interfaces) TryCapture(iface);
        return list;

        /// <summary>
        /// Tries to capture the given interface as an explicit one.
        /// </summary>
        bool TryCapture(INamedTypeSymbol iface)
        {
            var found = false;

            // First its childs...
            foreach (var child in iface.Interfaces) if (TryCapture(child)) found = true;

            // If no child, then maybe this interface by itself...
            if (!found)
            {
                if (iface.FindCloneableAttribute(true, out _) ||
                    iface.FindMethod(true, out _))
                    found = true;
            }

            // If found, add to the list...
            if (found)
            {
                var temp = list.Find(x => comparer.Equals(x, iface));
                if (temp is null) list.Add(iface);
            }

            // Finishing...
            return found;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Emits the appropriate documentation.
    /// </summary>
    void EmitDocumentation(CodeBuilder cb) => cb.AppendLine($$"""
        /// <summary>
        /// <inheritdoc cref="ICloneable.Clone"/>
        /// </summary>
        {{CloneGenerator.AttributeDoc}}
        """);
}
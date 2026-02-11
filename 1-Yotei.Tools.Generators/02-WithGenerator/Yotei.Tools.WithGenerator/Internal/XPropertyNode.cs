namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XPropertyNode : PropertyNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XPropertyNode(IPropertySymbol symbol) : base(symbol) { }

    /// <summary>
    /// Determines if this instance is built for an inherited member, or not.
    /// </summary>
    public bool IsInherited { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (Host.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); return false; }
        if (Symbol.IsIndexer) { Symbol.ReportError(TreeError.IndexerNotSupported, context); return false; }
        if (!Symbol.HasGetter) { Symbol.ReportError(TreeError.NoGetter, context); return false; }
        if (!Symbol.HasSetter && !Host.IsInterface) { Symbol.ReportError(TreeError.NoSetter, context); return false; }

        return base.Validate(context);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        // Intercepting explicitly implemented...
        if (FindMethodAt(Host) != null) return;

        // Capturing working data...
        if (!CaptureWorkingData(context)) return;

        // Dispatching...
        if (Host.IsInterface) EmitHostInterface(context, cb);
        else if (Host.IsAbstract) EmitHostAbstract(context, cb);
        else EmitHostRegular(context, cb);
    }

    /// <summary>
    /// Captures working data at emit time. Returns true if generation can continue, or false
    /// otherwise.
    /// </summary>
    bool CaptureWorkingData(SourceProductionContext context)
    {
        // Just 1 attribute is needed and allowed...
        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); return false; }
        if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); return false; }

        // HIGH: here I am...

        // Finishing...
        MethodName = $"With{Symbol.Name}";
        ArgumentName = $"value_{Symbol.Name}";
        return true;
    }

    string MethodName;
    string ArgumentName;

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an interface.
    /// </summary>
    void EmitHostInterface(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is an abstract type.
    /// </summary>
    void EmitHostAbstract(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked when the host is a regular type.
    /// </summary>
    void EmitHostRegular(SourceProductionContext context, CodeBuilder cb)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has a property compatible with this element.
    /// </summary>
    IPropertySymbol? FindMemberAt(INamedTypeSymbol type)
        => type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
        x.Name == Symbol.Name);

    /// <summary>
    /// Determines if the given type has a decorated property compatible with this element.
    /// </summary>
    IPropertySymbol? FindDecoratedMemberAt(INamedTypeSymbol type)
        => type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x =>
        x.Name == Symbol.Name && (
        x.HasAttributes(typeof(WithAttribute)) || x.HasAttributes(typeof(WithAttribute<>))));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has a method compatible with this element.
    /// </summary>
    IMethodSymbol? FindMethodAt(INamedTypeSymbol type)
        => type.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x =>
        x.Name == MethodName &&
        x.Parameters.Length == 1 &&
        Symbol.Type.IsAssignableTo(x.Parameters[0].Type));
}
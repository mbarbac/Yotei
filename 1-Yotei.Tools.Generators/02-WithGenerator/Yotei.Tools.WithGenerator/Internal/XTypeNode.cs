namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        var r = base.Validate(context);

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }
        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
        if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.EmitCore(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture inherited properties, if any.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        // Tries to capture inherited members at the given type's level...
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(static x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith(nameof(WithAttribute))))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                if (temp is null)
                {
                    var node = new XPropertyNode(member) { IsInherited = true, ParentNode = this };
                    ChildProperties.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture inherited fields, if any.
    /// </summary>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        // Tries to capture inherited members at the given type's level...
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(static x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith(nameof(WithAttribute))))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (temp is null)
                {
                    var node = new XFieldNode(member) { IsInherited = true, ParentNode = this };
                    ChildFields.Add(node);
                }
            }
        }
    }
}
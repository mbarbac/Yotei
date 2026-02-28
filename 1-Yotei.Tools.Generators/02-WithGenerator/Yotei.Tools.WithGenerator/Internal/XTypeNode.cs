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
    AttributeData Attribute = default!;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        if (Symbol.IsRecord) { Symbol.ReportError(TreeError.RecordsNotSupported, context); r = false; }

        if (Attributes.Count == 0) { Symbol.ReportError(TreeError.NoAttributes, context); r = false; }
        else if (Attributes.Count > 1) { Symbol.ReportError(TreeError.TooManyAttributes, context); r = false; }
        else Attribute = Attributes[0];

        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool OnEmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        var r = base.OnEmitCore(context, cb);
        if (r)
        {
            CaptureProperties();
            CaptureFields();
        }
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture inherited members, if any.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        /// <summary>
        /// Tries to capture inherited members from the given base type...
        /// </summary>
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().ToDebugArray();
            foreach (var member in members)
            {
                if (member.HasWithAttribute(out _))
                {
                    var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                    if (temp == null)
                    {
                        var node = new XPropertyNode(member) { ParentNode = this, IsInherited = true };
                        ChildProperties.Add(node);
                    }
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture inherited members, if any.
    /// </summary>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes) TryCaptureAt(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureAt(type);

        /// <summary>
        /// Tries to capture inherited members from the given base type...
        /// </summary>
        void TryCaptureAt(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().ToDebugArray();
            foreach (var member in members)
            {
                if (member.HasWithAttribute(out _))
                {
                    var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                    if (temp == null)
                    {
                        var node = new XFieldNode(member) { ParentNode = this, IsInherited = true };
                        ChildFields.Add(node);
                    }
                }
            }
        }
    }
}
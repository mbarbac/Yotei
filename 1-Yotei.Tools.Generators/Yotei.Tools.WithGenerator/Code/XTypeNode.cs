namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XTypeNode : TypeNode
{
    public XTypeNode(Node parent, TypeCandidate candidate) : base(parent, candidate) { }
    public XTypeNode(Node parent, ITypeSymbol symbol) : base(parent, symbol) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!base.Validate(context)) return false;
        if (!Symbol.ValidateNotRecord(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> Before that, this method captures the remaining inherited members that the
    /// type under consideration does not explicitly decorate, but only if the type itself is
    /// decorated.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        // Capturing inherited members...
        if (Symbol.HasAttributes(WithGeneratorAttr.LongName))
        {
            CaptureProperties();
            CaptureFields();
        }

        // Resuming...
        base.Print(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked at print-time to capture the remaining inherited properties not captured yet.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures the members not captured yet from the given type.
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var found = ChildProperties.Contains(x => x.Symbol.Name == member.Name);
                if (!found)
                {
                    var node = new XPropertyNode(this, member);
                    ChildProperties.Add(node);
                }
            }
        }
    }

    /// <summary>
    /// Invoked at print-time to capture the remaining inherited fields not captured yet.
    /// </summary>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures the members not captured yet from the given type.
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var found = ChildFields.Contains(x => x.Symbol.Name == member.Name);
                if (!found)
                {
                    var node = new XFieldNode(this, member);
                    ChildFields.Add(node);
                }
            }
        }
    }
}
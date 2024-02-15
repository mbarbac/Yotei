namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;

        if (!context.TypeIsNotRecord(Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// Before calling the base implementation, this override tries to capture the members that
    /// are decorated in the inheritance chain, but not in this type.
    /// </remarks>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.Print(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the members decorated in the inheritance chain, but not in this type.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures the member from the given type, if needed...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.HasAttributes(WithGeneratorAttr.LongName));

            foreach (var member in members)
            {
                var index = PropertyChildren.IndexOf(x => x.Symbol.Name == member.Name);
                if (index >= 0) continue;

                var node = new XPropertyNode(this, member);
                PropertyChildren.Add(node);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the members decorated in the inheritance chain, but not in this type.
    /// </summary>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures the member from the given type, if needed...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.HasAttributes(WithGeneratorAttr.LongName));

            foreach (var member in members)
            {
                var index = FieldChildren.IndexOf(x => x.Symbol.Name == member.Name);
                if (index >= 0) continue;

                var node = new XFieldNode(this, member);
                FieldChildren.Add(node);
            }
        }
    }
}
namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode(
    INode parent, INamedTypeSymbol symbol) : TypeNode(parent, symbol)
{
    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!context.TypeIsNotRecord(Symbol)) return false;

        if (!base.OnValidate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// When the attribute decorates a type, this is an instruction to capture the members that
    /// may be decorated in the inheritance chain, but not implemented in this type. This way
    /// we will reimplement them without needing an expliciting decoration in each member.
    /// </remarks>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the inherited members that are not decorated or implemented in this type.
    /// </summary>
    void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures at the given type level...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.HasAttributes(WithGeneratorAttr.LongName));

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                if (temp == null)
                {
                    var node = new XPropertyNode(this, member);
                    ChildProperties.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the inherited members that are not decorated or implemented in this type.
    /// </summary>
    void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures at the given type level...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.HasAttributes(WithGeneratorAttr.LongName));

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (temp == null)
                {
                    var node = new XFieldNode(this, member);
                    ChildFields.Add(node);
                }
            }
        }
    }
}
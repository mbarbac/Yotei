namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode(INode parent, INamedTypeSymbol symbol) : TypeNode(parent, symbol)
{
    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (Symbol.IsRecord) { context.TypeIsRecord(Symbol); return false; }

        if (!base.OnValidate(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// Before emitting the source code of this node in the hierarchy, we populate the members
    /// that are inherited but not decorated or implemented in this type, so that they can be
    /// emitted in its turn.
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
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName));

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
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName));

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
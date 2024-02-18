namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode(
    SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
    : TypeNode(model, syntax, symbol)
{
    public List<XProperty> InheritProperties { get; } = [];
    public List<XField> InheritFields { get; } = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        if (!base.OnValidate(context)) return false;
        if (!context.TypeIsNotRecord(Syntax)) return false;

        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// <remarks>
    /// Before calling the base implementation, we will capture the collections of inherited
    /// members that are decorated in the inheritance chain, but not in this type.
    /// </remarks>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties(context);
        CaptureFields(context);

        base.Emit(context, cb);

        foreach (var member in InheritProperties) member.Emit(context, cb);
        foreach (var member in InheritFields) member.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the inherited members that are not decorated in this type.
    /// </summary>
    void CaptureProperties(SourceProductionContext context)
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures the members of the given type...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.HasAttributes(WithGeneratorAttr.LongName));

            InheritProperties.Clear();
            foreach (var member in members)
            {
                var node = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                if (node != null) continue;

                var temp = InheritProperties.Find(x => x.Symbol.Name == member.Name);
                if (temp != null) continue;

                var item = new XProperty(member);
                if (!item.Validate(context)) continue;

                InheritProperties.Add(item);
            }
        }
    }

    /// <summary>
    /// Captures the inherited members that are not decorated in this type.
    /// </summary>
    void CaptureFields(SourceProductionContext context)
    {
        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures the members of the given type...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.HasAttributes(WithGeneratorAttr.LongName));

            InheritFields.Clear();
            foreach (var member in members)
            {
                var node = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (node != null) continue;

                var temp = InheritFields.Find(x => x.Symbol.Name == member.Name);
                if (temp != null) continue;
                
                var item = new XField(member);
                if (!item.Validate(context)) continue;

                InheritFields.Add(item);
            }
        }
    }
}
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
        // Base validations...
        if (!base.Validate(context)) return false;
        if (!ValidateNotRecord(context, Symbol)) return false;

        // Passed...
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        // Capturing the remaining inherited members...
        if (Symbol.HasAttributes(WithGeneratorAttr.LongName))
        {
            CaptureInheritedProperties();
            CaptureInheritedFields();
        }

        // Resuming the standard flow...
        base.Print(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked (at print time) to capture the remaining inherited properties, if any.
    /// </summary>
    public void CaptureInheritedProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) IdentifyAt(type);
        foreach (var iface in Symbol.AllInterfaces) IdentifyAt(iface);

        // Identify inherited members on the base type or interface, not yet considered...
        void IdentifyAt(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Contains(x => x.Symbol.Name == member.Name);
                if (!temp) Capture(member, Symbol);
            }
        }

        // Captures the given member...
        bool Capture(IPropertySymbol member, ITypeSymbol type)
        {
            var item = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x => x.Name == member.Name);
            if (item != null)
            {
                var node = new XPropertyNode(this, item);
                ChildProperties.Add(node);
                return true;
            }

            foreach (var child in type.AllBaseTypes()) if (Capture(member, child)) return true;
            foreach (var iface in type.AllInterfaces) if (Capture(member, iface)) return true;
            return false;
        }
    }

    /// <summary>
    /// Invoked (at print time) to capture the remaining inherited field, if any.
    /// </summary>
    public void CaptureInheritedFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) IdentifyAt(type);
        foreach (var iface in Symbol.AllInterfaces) IdentifyAt(iface);

        // Identify inherited members on the base type or interface, not yet considered...
        void IdentifyAt(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Contains(x => x.Symbol.Name == member.Name);
                if (!temp) Capture(member, Symbol);
            }
        }

        // Captures the given member...
        bool Capture(IFieldSymbol member, ITypeSymbol type)
        {
            var item = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(x => x.Name == member.Name);
            if (item != null)
            {
                var node = new XFieldNode(this, item);
                ChildFields.Add(node);
                return true;
            }

            foreach (var child in type.AllBaseTypes()) if (Capture(member, child)) return true;
            foreach (var iface in type.AllInterfaces) if (Capture(member, iface)) return true;
            return false;
        }
    }
}
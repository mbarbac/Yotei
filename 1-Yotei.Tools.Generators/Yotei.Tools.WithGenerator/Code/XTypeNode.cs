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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        // Capturing the remaining inherited members...
        if (Symbol.HasAttributes(WithGeneratorAttr.LongName))
        {
            CaptureProperties();
            CaptureFields();
        }

        // Resuming the standard flow...
        base.Print(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked (at print time) to capture the remaining inherited properties, if any.
    /// </summary>
    public void CaptureProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) Identify(type);
        foreach (var iface in Symbol.AllInterfaces) Identify(iface);

        /// <summary>
        /// Invoked to identify the remaining inherited members not yet captured.
        /// </summary>
        void Identify(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Contains(x => x.Symbol.Name == member.Name);
                if (!temp) Capture(Symbol, member);
            }
        }

        /// <summary>
        /// Invoked to capture the given member of the given type.
        /// </summary>
        bool Capture(ITypeSymbol type, IPropertySymbol member)
        {
            var item = type.GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(x => x.Name == member.Name);

            if (item != null)
            {
                var node = new XPropertyNode(this, item);
                ChildProperties.Add(node);
                return true;
            }

            foreach (var child in type.AllBaseTypes()) if (Capture(child, member)) return true;
            foreach (var iface in type.AllInterfaces) if (Capture(iface, member)) return true;
            return false;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked (at print time) to capture the remaining inherited fields, if any.
    /// </summary>
    public void CaptureFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) Identify(type);
        foreach (var iface in Symbol.AllInterfaces) Identify(iface);

        /// <summary>
        /// Invoked to identify the remaining inherited members not yet captured.
        /// </summary>
        void Identify(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(WithGeneratorAttr.LongName))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Contains(x => x.Symbol.Name == member.Name);
                if (!temp) Capture(Symbol, member);
            }
        }

        /// <summary>
        /// Invoked to capture the given member of the given type.
        /// </summary>
        bool Capture(ITypeSymbol type, IFieldSymbol member)
        {
            var item = type.GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(x => x.Name == member.Name);

            if (item != null)
            {
                var node = new XFieldNode(this, item);
                ChildFields.Add(node);
                return true;
            }

            foreach (var child in type.AllBaseTypes()) if (Capture(child, member)) return true;
            foreach (var iface in type.AllInterfaces) if (Capture(iface, member)) return true;
            return false;
        }
    }
}
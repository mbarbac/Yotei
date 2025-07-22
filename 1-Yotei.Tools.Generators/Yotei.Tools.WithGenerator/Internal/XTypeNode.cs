namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    /*
    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        // Base validations...
        if (!base.Validate(context)) return false;

        // Other validations...
        if (Symbol.IsRecord)
        {
            TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
            return false;
        }

        // Finishing...
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureInheritedProperties();
        CaptureInheritedFields();

        base.Emit(context, cb);
    }

    /// <summary>
    /// Invoked to capture the inherited members that have not been captured yet, because this
    /// host type is decorated with the <see cref="InheritWithsAttribute"/> attribute. Captured
    /// members will have their <see cref="PropertyNode.Candidate"/> property set to null.
    /// </summary>
    void CaptureInheritedProperties()
    {
        var comparer = SymbolComparer.Default;

        foreach (var type in Symbol.AllBaseTypes()) TryCapture(type);
        foreach (var type in Symbol.AllInterfaces) TryCapture(type);

        // Tries to capture members at the given symbol's level only.
        void TryCapture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => comparer.Equals(x.Symbol, member));
                if (temp == null)
                {
                    var node = new XPropertyNode(this, member);
                    ChildProperties.Add(node);
                }
            }
        }
    }

    /// <summary>
    /// Invoked to capture the inherited members that have not been captured yet, because this
    /// host type is decorated with the <see cref="InheritWithsAttribute"/> attribute. Captured
    /// members will have their <see cref="FieldNode.Candidate"/> property set to null.
    /// </summary>
    void CaptureInheritedFields()
    {
        var comparer = SymbolComparer.Default;

        foreach (var type in Symbol.AllBaseTypes()) TryCapture(type);
        foreach (var type in Symbol.AllInterfaces) TryCapture(type);

        // Tries to capture members at the given symbol's level only.
        void TryCapture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => comparer.Equals(x.Symbol, member));
                if (temp == null)
                {
                    var node = new XFieldNode(this, member);
                    ChildFields.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the <see cref="InheritWithsAttribute"/> attribute applied to the given type, or
    /// or null if any. The base types and interfaces of the type are also searched, if such is
    /// explicitly requested.
    /// </summary>
    static internal AttributeData? FindInheritsWithAttribute(
        INamedTypeSymbol type, bool chain = false, bool ifaces = false)
    {
        var at = type.GetAttributes(typeof(InheritWithsAttribute)).FirstOrDefault();

        if (at == null && chain)
        {
            foreach (var child in type.AllBaseTypes())
            {
                at = FindInheritsWithAttribute(child);
                if (at != null) break;
            }
        }

        if (at == null && ifaces)
        {
            foreach (var child in type.AllInterfaces)
            {
                at = FindInheritsWithAttribute(child);
                if (at != null) break;
            }
        }

        return at;
    }*/
}
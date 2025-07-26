namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }


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
    /// If we are here the type is decorated with the <see cref="InheritWithsAttribute"/>, so
    /// we need to find the decorated members that are inherited but not reimplemented in the
    /// type itself. Note that at this moment the hierarchy of nodes is already captured, so
    /// we can intercept duplications.
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureInheritedProperties();
        CaptureInheritedFields();

        base.Emit(context, cb);
    }

    /// <summary>
    /// Invoked to capture the inherited members that have not been captured yet. Captured members
    /// will have their <see cref="PropertyNode.Candidate"/> property set to null.
    /// </summary>
    void CaptureInheritedProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) TryCapture(type);
        foreach (var type in Symbol.AllInterfaces) TryCapture(type);

        // Tries to capture members at the given symbol's level, but only if there is not yet a
        // member captured with the same name.
        void TryCapture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                if (temp == null)
                {
                    var node = new XPropertyNode(this, member) { IsInherited = true };
                    ChildProperties.Add(node);
                }
            }
        }
    }

    /// <summary>
    /// Invoked to capture the inherited members that have not been captured yet. Captured members
    /// will have their <see cref="FieldNode.Candidate"/> property set to null.
    /// </summary>
    void CaptureInheritedFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) TryCapture(type);
        foreach (var type in Symbol.AllInterfaces) TryCapture(type);

        // Tries to capture members at the given symbol's level, but only if there is not yet a
        // member captured with the same name.
        void TryCapture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (temp == null)
                {
                    var node = new XFieldNode(this, member) { IsInherited = true };
                    ChildFields.Add(node);
                }
            }
        }
    }
}
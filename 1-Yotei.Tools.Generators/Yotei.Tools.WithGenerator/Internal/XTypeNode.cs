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
        bool r = true;

        if (!base.Validate(context)) r = false;
        if (!ValidateNoRecord(context)) r = false;

        return r;
    }

    // Custom validations...
    bool ValidateNoRecord(SourceProductionContext context)
    {
        if (!Symbol.IsRecord) return true;

        TreeDiagnostics.RecordsNotSupported(Symbol).Report(context);
        return false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// When this method is invoked, the hierarchy is already built, so we can confidently find
    /// duplicates and prevent adding them twice.
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureInheritedProperties();
        CaptureInheritedFields();

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited members that have not been captured yet.
    /// <br/> Inherited members have their 'Candidate' property set to null.
    /// </summary>
    void CaptureInheritedProperties()
    {
        foreach (var type in Symbol.AllBaseTypes()) TryCaptureFrom(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureFrom(type);

        /// <summary>
        /// Tries to capture the members inherited from the given type, provided that have not
        /// been captured yet.
        /// </summary>
        void TryCaptureFrom(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>().Where(x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith("WithAttribute")))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => x.Symbol.Name == member.Name);
                if (temp is null)
                {
                    var node = new XPropertyNode(this, member);
                    ChildProperties.Add(node);
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited members that have not been captured yet.
    /// <br/> Inherited members have their 'Candidate' property set to null.
    /// </summary>
    void CaptureInheritedFields()
    {
        foreach (var type in Symbol.AllBaseTypes()) TryCaptureFrom(type);
        foreach (var type in Symbol.AllInterfaces) TryCaptureFrom(type);

        /// <summary>
        /// Tries to capture the members inherited from the given type, provided that have not
        /// been captured yet.
        /// </summary>
        void TryCaptureFrom(INamedTypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.GetAttributes().Any(y =>
                y.AttributeClass is not null &&
                y.AttributeClass.Name.StartsWith("WithAttribute")))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => x.Symbol.Name == member.Name);
                if (temp is null)
                {
                    var node = new XFieldNode(this, member);
                    ChildFields.Add(node);
                }
            }
        }
    }
}
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
        var r = true;

        if (Symbol.IsRecord)
        {
            TreeDiagnostics.KindNotSupported(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureInheritedProperties();
        CaptureInheritedFields();

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited properties that have not been captured yet.
    /// </summary>
    void CaptureInheritedProperties()
    {
        var comparer = SymbolComparer.Empty;

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Capture members at the type's level...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IPropertySymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildProperties.Find(x => comparer.Equals(x.Symbol, member));
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
    /// Invoked to capture the inherited properties that have not been captured yet.
    /// </summary>
    void CaptureInheritedFields()
    {
        var comparer = SymbolComparer.Empty;

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Capture members at the type's level...
        void Capture(ITypeSymbol type)
        {
            var members = type.GetMembers().OfType<IFieldSymbol>()
                .Where(x => x.HasAttributes(typeof(WithAttribute)))
                .ToDebugArray();

            foreach (var member in members)
            {
                var temp = ChildFields.Find(x => comparer.Equals(x.Symbol, member));
                if (temp is null)
                {
                    var node = new XFieldNode(this, member);
                    ChildFields.Add(node);
                }
            }
        }
    }
}
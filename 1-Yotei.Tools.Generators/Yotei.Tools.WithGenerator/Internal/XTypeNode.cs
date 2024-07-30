namespace Yotei.Tools.WithGenerator;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool IsSupportedKind()
    {
        if (Symbol.IsRecord) return false;
        return base.IsSupportedKind();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        CaptureProperties();
        CaptureFields();

        base.Emit(context, cb);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited properties.
    /// </summary>
    void CaptureProperties()
    {
        var comparer = SymbolComparer.Empty;

        foreach(var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures members at the type's level...
        void Capture(ITypeSymbol type)
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the inherited fields.
    /// </summary>
    void CaptureFields()
    {
        var comparer = SymbolComparer.Empty;

        foreach (var type in Symbol.AllBaseTypes()) Capture(type);
        foreach (var type in Symbol.AllInterfaces) Capture(type);

        // Captures members at the type's level...
        void Capture(ITypeSymbol type)
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
}
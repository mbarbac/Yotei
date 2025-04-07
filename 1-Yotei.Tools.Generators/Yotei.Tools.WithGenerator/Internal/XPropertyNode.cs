namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="PropertyNode"/>
internal class XPropertyNode : PropertyNode
{
    public XPropertyNode(TypeNode parent, IPropertySymbol symbol) : base(parent, symbol) { }
    public XPropertyNode(TypeNode parent, PropertyCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        var r = true;

        if (Host.IsRecord)
        {
            TreeDiagnostics.KindNotSupported(Host).Report(context);
            r = false;
        }
        if (Symbol.IsIndexer)
        {
            TreeDiagnostics.IndexerNotSupported(Symbol).Report(context);
            r = false;
        }
        if (!Symbol.HasGetter())
        {
            TreeDiagnostics.NoGetter(Symbol).Report(context);
            r = false;
        }
        if (!Host.IsInterface() && !Symbol.HasSetterOrInit())
        {
            TreeDiagnostics.NoSetter(Symbol).Report(context);
            r = false;
        }

        if (!base.Validate(context)) r = false;

        return r;
    }
}
namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="FieldNode"/>
internal class XFieldNode : FieldNode
{
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }
    public XFieldNode(TypeNode parent, FieldCandidate candidate) : base(parent, candidate) { }

    INamedTypeSymbol Host => ParentNode.Symbol;
    string MethodName => $"With{Symbol.Name}";
    string ArgumentName => $"v_{Symbol.Name}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SourceProductionContext context)
    {
        bool r = true;

        // Base validations...
        if (!base.Validate(context)) r = false;

        // Other validations...
        if (Host.IsRecord) { TreeDiagnostics.RecordsNotSupported(Host).Report(context); r = false; }
        if (!Host.IsInterface() && !Symbol.IsWrittable()) { TreeDiagnostics.NotWrittable(Symbol).Report(context); r = false; }
        if (!CanUseReturnInterface()) { WithDiagnostics.InvalidReturnInterface(Host).Report(context); return false; }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    bool CanUseReturnInterface() => true;
}
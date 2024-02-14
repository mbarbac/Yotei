namespace Yotei.Tools.InheritGenerator;

// ========================================================
/// <inheritdoc/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    InheritElement[] Elements { get; set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        Elements = InheritAttr.GetElements(Symbol).ToArray();
        base.Print(context, cb);
    }

    /// <inheritdoc/>
    protected override string GetTypeName()
    {
        var name = base.GetTypeName();
        return name;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnPrint(SourceProductionContext context, CodeBuilder cb)
    {
        base.OnPrint(context, cb);
    }
}
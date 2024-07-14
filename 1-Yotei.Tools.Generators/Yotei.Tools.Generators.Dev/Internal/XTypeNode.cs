namespace Yotei.Tools.Generators.Dev;

// =========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode : TypeNode
{
    public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
    public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

    protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
    {
        var item = Candidate!.Attributes[0];
        var options = EasyNameOptions.Full;
        var name = item!.EasyName(options);

        //var item = Symbol.GetMembers().OfType<IFieldSymbol>().First();
        //var options = EasyNameOptions.Full;
        //var name = item!.EasyName(options);
    }
}
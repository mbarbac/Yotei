namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XFieldNode : FieldNode
{
    public XFieldNode(TypeNode parent, IFieldSymbol symbol) : base(parent, symbol) { }
    public XFieldNode(TypeNode parent, FieldCandidate candidate) : base(parent, candidate) { }
}
namespace Yotei.Generators.MemberWith;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
internal class XField : Tree.FieldNode
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    /// <param name="model"></param>
    public XField(
        Tree.ITypeNode parent,
        FieldDeclarationSyntax syntax, IFieldSymbol symbol, SemanticModel model)
        : base(parent, syntax, symbol, model) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override bool Validate(SourceProductionContext context) => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Print(SourceProductionContext context, CodeBuilder cb) { }
}
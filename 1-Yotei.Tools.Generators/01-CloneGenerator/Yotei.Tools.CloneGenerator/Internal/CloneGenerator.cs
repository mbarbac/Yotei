namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : TreeGenerator
{
#if DEBUG_CLONE_GENERATOR
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool LaunchDebugger => true;
#endif

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>),];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TypeNode CreateNode(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XTypeNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }
}
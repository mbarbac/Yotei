namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG_WITH_GENERATOR_
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool LaunchDebugger => true;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(InheritsWithAttribute),
        typeof(InheritsWithAttribute<>),
    ];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> PropertyAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>),
    ];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> FieldAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>),
    ];

    // ----------------------------------------------------

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override PropertyNode CreateNode(
        IPropertySymbol symbol,
        BasePropertyDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XPropertyNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override FieldNode CreateNode(
        IFieldSymbol symbol,
        FieldDeclarationSyntax? syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XFieldNode(symbol);
        if (syntax != null) item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }
}
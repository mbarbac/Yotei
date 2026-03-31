
namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class InvariantGenerator : TreeGenerator
{
#if DEBUG_INVARIANT_GENERATOR
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
        typeof(IInvariantBagAttribute),
        typeof(IInvariantBagAttribute<>),
        typeof(IInvariantListAttribute),
        typeof(IInvariantListAttribute<>),
        typeof(IInvariantListAttribute<,>),

        typeof(InvariantBagAttribute),
        typeof(InvariantBagAttribute<>),
        typeof(InvariantListAttribute),
        typeof(InvariantListAttribute<>),
        typeof(InvariantListAttribute<,>),
    ];

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
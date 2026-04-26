namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class InvariantGenerator : TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        // Base method first...
        base.OnInitialize(context);

        // Marker attributes...
        AddInitializationResource(context, "InvariantGenerator/Public/Api/IInvariantBagAttribute.cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Api/IInvariantBagAttribute[T].cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Api/IInvariantListAttribute.cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Api/IInvariantListAttribute[K,T].cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Api/IInvariantListAttribute[T].cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Code/InvariantBagAttribute.cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Code/InvariantBagAttribute[T].cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Code/InvariantListAttribute.cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Code/InvariantListAttribute[K,T].cs", "Markers");
        AddInitializationResource(context, "InvariantGenerator/Public/Code/InvariantListAttribute[T].cs", "Markers");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(IInvariantBagAttribute),
        typeof(IInvariantBagAttribute<>),
        typeof(IInvariantListAttribute),
        typeof(IInvariantListAttribute<,>),
        typeof(IInvariantListAttribute<>),
        typeof(InvariantBagAttribute),
        typeof(InvariantBagAttribute<>),
        typeof(InvariantListAttribute),
        typeof(InvariantListAttribute<,>),
        typeof(InvariantListAttribute<>),];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    protected override TypeNode CreateNode(
        INamedTypeSymbol symbol,
        BaseTypeDeclarationSyntax syntax,
        IEnumerable<AttributeData> attributes,
        SemanticModel model)
    {
        var item = new XTypeNode(symbol); // for this generator!
        item.SyntaxNodes.Add(syntax);
        item.Attributes.AddRange(attributes);
        return item;
    }
}
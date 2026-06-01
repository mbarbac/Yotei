namespace Yotei.ORM.InvariantGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class InvariantGenerator : TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        base.OnInitialize(context);
        AddLocalResource(context, "Yotei.ORM.Generators/Public/IInvariantBagAttribute.cs", "Markers/IInvariantBagAttribute.cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/IInvariantBagAttribute[T].cs", "Markers/IInvariantBagAttribute[T].cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/IInvariantListAttribute.cs", "Markers/IInvariantListAttribute.cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/IInvariantListAttribute[K,T].cs", "Markers/IInvariantListAttribute[K,T].cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/IInvariantListAttribute[T].cs", "Markers/IInvariantListAttribute[T].cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/InvariantBagAttribute.cs", "Markers/InvariantBagAttribute.cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/InvariantBagAttribute[T].cs", "Markers/InvariantBagAttribute[T].cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/InvariantListAttribute.cs", "Markers/InvariantListAttribute.cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/InvariantListAttribute[K,T].cs", "Markers/InvariantListAttribute[K,T].cs");
        AddLocalResource(context, "Yotei.ORM.Generators/Public/InvariantListAttribute[T].cs", "Markers/InvariantListAttribute[T].cs");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override string? NullabilityHelpersNamespace => "Yotei.ORM.InvariantGenerator";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(IInvariantBagAttribute), typeof(IInvariantBagAttribute<>),
        typeof(IInvariantListAttribute),
        typeof(IInvariantListAttribute<>), typeof(IInvariantListAttribute<,>),
        typeof(InvariantBagAttribute), typeof(InvariantBagAttribute<>),
        typeof(InvariantListAttribute),
        typeof(InvariantListAttribute<>), typeof(InvariantListAttribute<,>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected override TypeNode CreateNode(INamedTypeSymbol symbol) => new XTypeNode(symbol);
}
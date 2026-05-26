namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class WithGenerator : TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        base.OnInitialize(context);
        AddLocalResource(context, "Public/WithAttribute.cs", "Markers/WithAttribute.cs");
        AddLocalResource(context, "Public/WithAttribute[T].cs", "Markers/WithAttribute[T].cs");
        AddLocalResource(context, "Public/InheritsWithAttribute.cs", "Markers/InheritsWithAttribute.cs");
        AddLocalResource(context, "Public/InheritsWithAttribute[T].cs", "Markers/InheritsWithAttribute[T].cs");
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(InheritsWithAttribute),
        typeof(InheritsWithAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> PropertyAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> FieldAttributes { get; } = [
        typeof(WithAttribute),
        typeof(WithAttribute<>)];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected override TypeNode CreateNode(INamedTypeSymbol symbol) => new XTypeNode(symbol);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected override PropertyNode CreateNode(IPropertySymbol symbol) => new XPropertyNode(symbol);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected override FieldNode CreateNode(IFieldSymbol symbol) => new XFieldNode(symbol);
}
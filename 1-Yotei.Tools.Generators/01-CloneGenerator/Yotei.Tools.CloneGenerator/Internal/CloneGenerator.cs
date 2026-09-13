namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : TreeGenerator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        base.OnInitialize(context);
        AddLocalResource(context, "Yotei.Tools.CloneGenerator/Public/CloneableAttribute.cs", "Markers/CloneableAttribute.cs");
        AddLocalResource(context, "Yotei.Tools.CloneGenerator/Public/CloneableAttribute[T].cs", "Markers/CloneableAttribute[T].cs");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override string? NullabilityHelpersNamespace => "Yotei.Tools.CloneGenerator";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    protected override TypeNode CreateNode(INamedTypeSymbol symbol) => new XTypeNode(symbol);
}
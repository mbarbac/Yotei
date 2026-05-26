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
}
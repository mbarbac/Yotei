namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class CloneGenerator : TreeGenerator
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
        AddInitializationResource(context, "Public.CloneableAttribute.cs", "Markers");
        AddInitializationResource(context, "Public.CloneableAttribute[T].cs", "Markers");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>),];
}
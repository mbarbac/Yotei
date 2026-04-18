using System.ComponentModel.Design;

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
}
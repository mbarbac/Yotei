namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class CloneGenerator : TreeGenerator
{
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        var name = "a.b.c.name";
        var usefolder = false;
        var reverse = false;
        name = NormalizeFileName(name, usefolder, reverse);
    }
}
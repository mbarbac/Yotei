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
        var nspace = GetType().Namespace;
        var fname = "CloneableAttribute.cs";
        var source = ReadSourceContents(nspace, fname);
        AddSourceContents(context, nspace, true, fname, source);
    }
}
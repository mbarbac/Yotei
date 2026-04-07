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
        //var folder = !EmitFilesInFolders ? null : GetType().Namespace;
        //DoMarker("CloneableAttribute.cs");
        //DoMarker("CloneableAttribute[T].cs");

        //// Emits the market attribute...
        //void DoMarker(string name)
        //{
        //    var source = ReadSourceContents($"Public\\{name}");
        //    AddSourceContents(context, folder, name, source);
        //}
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>),];
}
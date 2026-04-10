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
    protected override bool EmitFilesInFolders => false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        string rname;
        var rfolder = "Public/";
        var nspace = GetType().Namespace;

        rname = "CloneableAttribute.cs";
        AddResourceContents(context, $"{rfolder}{rname}", nspace, rname, EmitFilesInFolders);

        rname = "CloneableAttribute[T].cs";
        AddResourceContents(context, $"{rfolder}{rname}", nspace, rname, EmitFilesInFolders);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>),];
}
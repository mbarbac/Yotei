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
    protected override void OnEmitContext(ref TreeContext context)
    {
        // Base method first...
        base.OnEmitContext(ref context);

        // Marker attributes...
        var usefolders = context.Options.UseFileFolders;
        var reversenames = context.Options.ReverseFileNames;
        var rfolder = "Public";
        var nspace = GetType().Namespace;

        ReadAndEmitResource(rfolder, "CloneableAttribute.cs", nspace, usefolders, reversenames, context.Context);
        ReadAndEmitResource(rfolder, "CloneableAttribute[T].cs", nspace, usefolders, reversenames, context.Context);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override List<Type> TypeAttributes { get; } = [
        typeof(CloneableAttribute),
        typeof(CloneableAttribute<>),];
}
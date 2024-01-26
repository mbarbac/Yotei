namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc cref="TreeGenerator"/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : TreeGenerator
{
#if DEBUG_CLONE_GENERATOR
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool LaunchDebugger => true;
#endif

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        var nsName = typeof(CloneableAttr).Namespace;
        cb.AppendLine();
        cb.AppendLine(CloneableAttr.Code(nsName));

        cb.AppendLine();
        cb.AppendLine(YoteiGeneratedAttr.Code(nsName));

        var code = cb.ToString();
        var name = CloneableAttr.LongName + ".g.cs";
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] TypeAttributes => [CloneableAttr.LongName];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override TypeNode CreateNode(
        INode parent,
        TypeCandidate candidate) => new XTypeNode(parent, candidate);
}
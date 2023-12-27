namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc cref="BaseGenerator"/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : BaseGenerator
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

        var nsName = typeof(CloneGenerator).Namespace;
        cb.AppendLine();
        cb.AppendLine(CloneableAttr.Code(nsName));

        var code = cb.ToString();
        var name = "Attributes." + nsName + ".g.cs";
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
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}
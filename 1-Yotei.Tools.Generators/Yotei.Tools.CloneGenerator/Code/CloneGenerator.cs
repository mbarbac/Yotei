namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class CloneGenerator : TreeGenerator
{
#if DEBUG && DEBUG_CLONEGENERATOR
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override bool LaunchDebugger => true;
#endif

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine();
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
    protected override string[] TypeAttributes { get; } = [
        CloneableAttr.LongName,
    ];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override TypeNode CreateType(
        Node parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}
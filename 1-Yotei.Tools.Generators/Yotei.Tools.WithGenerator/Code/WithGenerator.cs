namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc cref="TreeGenerator"/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG_WITH_GENERATOR
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

        var nsName = typeof(WithGeneratorAttr).Namespace;
        cb.AppendLine();
        cb.AppendLine(WithGeneratorAttr.Code(nsName));

        var code = cb.ToString();
        var name = WithGeneratorAttr.LongName + ".g.cs";
        context.AddSource(name, code);
    }
}
namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class XUpcastGenerator : TreeGenerator
{
#if DEBUG && DEBUG_UPCAST_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif
    public static string CodeNamespace => typeof(XUpcastGenerator).Namespace;
    public static string YoteiGenerated => YoteiGeneratedAttr.GetDecorator(CodeNamespace);

    /// <inheritdoc/>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        cb.AppendLine();
        cb.AppendLine(UpcastAttr.Code(CodeNamespace));

        cb.AppendLine();
        cb.AppendLine(YoteiGeneratedAttr.Code(CodeNamespace));

        var code = cb.ToString();
        var name = UpcastAttr.LongName + ".g.cs";
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string[] TypeAttributes => [UpcastAttr.LongName];

    /// <inheritdoc/>
    public override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}
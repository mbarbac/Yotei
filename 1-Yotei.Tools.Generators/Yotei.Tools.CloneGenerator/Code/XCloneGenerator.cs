namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class XCloneGenerator : TreeGenerator
{
#if DEBUG && DEBUG_CLONE_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif
    public static string CodeNamespace => typeof(XCloneGenerator).Namespace;
    public static string GeneratedCode
        => GeneratedCodeAttr.GetDecorator(CodeNamespace, "Yotei", "CloneGenerator");

    /// <inheritdoc/>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        cb.AppendLine();
        cb.AppendLine(CloneableAttr.Code(CodeNamespace));

        cb.AppendLine();
        cb.AppendLine(GeneratedCodeAttr.Code(CodeNamespace));

        var code = cb.ToString();
        var name = CloneableAttr.LongName + ".g.cs";
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string[] TypeAttributes => [CloneableAttr.LongName];

    /// <inheritdoc/>
    public override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate.Symbol);
}
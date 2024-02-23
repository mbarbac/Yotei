namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class XWithGenerator : TreeGenerator
{
#if DEBUG && DEBUG_WITH_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif
    public static string CodeNamespace => typeof(XWithGenerator).Namespace;
    public static string YoteiGenerated => YoteiGeneratedAttr.GetDecorator(CodeNamespace);

    /// <inheritdoc/>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        cb.AppendLine();
        cb.AppendLine(WithGeneratorAttr.Code(CodeNamespace));

        cb.AppendLine();
        cb.AppendLine(YoteiGeneratedAttr.Code(CodeNamespace));

        var code = cb.ToString();
        var name = WithGeneratorAttr.LongName + ".g.cs";
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string[] TypeAttributes => [WithGeneratorAttr.LongName];

    /// <inheritdoc/>
    public override string[] PropertyAttributes => [WithGeneratorAttr.LongName];

    /// <inheritdoc/>
    public override string[] FieldAttributes => [WithGeneratorAttr.LongName];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override string GetFileName(ICandidate candidate) => base.GetFileName(candidate);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate.Symbol);

    /// <inheritdoc/>
    public override PropertyNode CreateNode(
        TypeNode parent, PropertyCandidate candidate) => new XPropertyNode(parent, candidate.Symbol);

    /// <inheritdoc/>
    public override FieldNode CreateNode(
        TypeNode parent, FieldCandidate candidate) => new XFieldNode(parent, candidate.Symbol);
}
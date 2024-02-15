namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG && DEBUG_WITH_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        var nsName = typeof(WithGeneratorAttr).Namespace;
        cb.AppendLine();
        cb.AppendLine(WithGeneratorAttr.Code(nsName));

        cb.AppendLine();
        cb.AppendLine(YoteiGeneratedAttr.Code(nsName));

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
    public override TypeNode CreateNode(
        INode parent,
        TypeCandidate candidate) => new XTypeNode(parent, candidate);

    /// <inheritdoc/>
    public override PropertyNode CreateNode(
        TypeNode parent,
        PropertyCandidate candidate) => new XPropertyNode(parent, candidate);

    /// <inheritdoc/>
    public override FieldNode CreateNode(
        TypeNode parent,
        FieldCandidate candidate) => new XFieldNode(parent, candidate);
}
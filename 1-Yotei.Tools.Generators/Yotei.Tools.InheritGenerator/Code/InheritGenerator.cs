
namespace Yotei.Tools.InheritGenerator;

// ========================================================
/// <inheritdoc cref="TreeGenerator"/>
[Generator(LanguageNames.CSharp)]
internal class InheritGenerator : TreeGenerator
{
#if DEBUG && DEBUG_INHERIT_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    /// <inheritdoc/>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        var nsName = typeof(InheritAttr).Namespace;

        cb.AppendLine();
        cb.AppendLine(InheritAttr.Code(nsName));

        cb.AppendLine();
        cb.AppendLine(IGenericIFace.Code(nsName));

        cb.AppendLine();
        cb.AppendLine(YoteiGeneratedAttr.Code(nsName));

        var code = cb.ToString();
        var name = InheritAttr.LongName + ".g.cs";
        context.AddSource(name, code);
    }

    /// <inheritdoc/>
    public override string[] TypeAttributes => [InheritAttr.LongName];

    /// <inheritdoc/>
    public override TypeNode CreateNode(
        INode parent,
        TypeCandidate candidate) => new XTypeNode(parent, candidate);
}
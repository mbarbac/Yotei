
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override string[] TypeAttributes => [CloneableAttr.LongName];

    /// <inheritdoc/>
    public override TypeNode CreateNode(
        SemanticModel model, TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        => new XTypeNode(model, syntax, symbol);
}
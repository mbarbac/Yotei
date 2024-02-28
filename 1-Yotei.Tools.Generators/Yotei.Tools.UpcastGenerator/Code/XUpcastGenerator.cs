
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
    public static string GeneratedCode
        => GeneratedCodeAttr.GetDecorator(CodeNamespace, "Yotei", "UpcastGenerator");

    /// <inheritdoc/>
    protected override void OnInitialized(IncrementalGeneratorPostInitializationContext context)
    {
        CodeBuilder cb = new();
        cb.AppendLine("#nullable enable");
        cb.AppendLine("using System;");

        cb.AppendLine();
        cb.AppendLine(UpcastInterface.Code(CodeNamespace));

        cb.AppendLine();
        cb.AppendLine(GeneratedCodeAttr.Code(CodeNamespace));

        var code = cb.ToString();
        var name = $"{nameof(UpcastInterface)}.{CodeNamespace}" + ".g.cs";
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool Validate(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if (node is not TypeDeclarationSyntax syntax) return false;
        if (syntax.BaseList == null) return false;

        foreach (var type in syntax.BaseList.Types)
        {
            if (type.Type is GenericNameSyntax named &&
                named.Arity == 1 && (
                named.Identifier.Text == "IUpcast" || named.Identifier.Text == "IUpcastEx"))
                return true;
        }
        return false;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);
}
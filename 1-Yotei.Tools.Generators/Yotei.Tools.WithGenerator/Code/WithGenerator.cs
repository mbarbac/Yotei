namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc cref="BaseGenerator"/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : BaseGenerator
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

        var nsName = typeof(WithGenerator).Namespace;
        cb.AppendLine();
        cb.AppendLine(WithGeneratorAttr.Code(nsName));

        var code = cb.ToString();
        var name = "Attributes." + nsName + ".g.cs";
        context.AddSource(name, code);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] TypeAttributes => [WithGeneratorAttr.LongName];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] PropertyAttributes => [WithGeneratorAttr.LongName];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] FieldAttributes => [WithGeneratorAttr.LongName];

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override TypeNode CreateNode(
        INode parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override PropertyNode CreateNode(
        TypeNode parent, PropertyCandidate candidate) => new XPropertyNode(parent, candidate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override FieldNode CreateNode(
        TypeNode parent, FieldCandidate candidate) => new XFieldNode(parent, candidate);
}
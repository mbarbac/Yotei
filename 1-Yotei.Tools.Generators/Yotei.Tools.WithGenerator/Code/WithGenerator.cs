namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : TreeGenerator
{
#if DEBUG && DEBUG_WITHGENERATOR
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
    protected override string[] TypeAttributes { get; } = [
        WithGeneratorAttr.LongName,
    ];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override string[] PropertyAttributes { get; } = [
        WithGeneratorAttr.LongName,
    ];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override string[] FieldAttributes { get; } = [
        WithGeneratorAttr.LongName,
    ];

    // ----------------------------------------------------

    /*
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public override TypeNode CreateType(
        Node parent, ITypeSymbol symbol) => new XTypeNode(parent, symbol);
    */

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override TypeNode CreateType(
        Node parent, TypeCandidate candidate) => new XTypeNode(parent, candidate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override PropertyNode CreateProperty(
        TypeNode parent, PropertyCandidate candidate) => new XPropertyNode(parent, candidate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override FieldNode CreateField(
        TypeNode parent, FieldCandidate candidate) => new XFieldNode(parent, candidate);
}
namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class WithGenerator : BaseGenerator
{
#if DEBUG_WITHGENERATOR
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
        cb.AppendLine();
        cb.AppendLine("using System;");

        var nsName = typeof(WithGenerator).Namespace;
        cb.AppendLine();
        cb.AppendLine(WithGeneratorAttr.Code(nsName));

        var code = cb.GetCode();
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
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override TypeNode CreateType(TypeCandidate candidate) => new XType(candidate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override PropertyNode CreateProperty(PropertyCandidate candidate) => new XProperty(candidate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public override FieldNode CreateField(FieldCandidate candidate) => new XField(candidate);
}
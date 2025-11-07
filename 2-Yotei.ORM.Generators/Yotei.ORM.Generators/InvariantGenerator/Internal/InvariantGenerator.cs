namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal partial class InvariantGenerator : CoreGenerator
{
#if INVARIANT_GENERATOR
    /// <inheritdoc/>
    protected override bool LaunchDebugger => true;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Type[] TypeAttributes { get; } = [
        typeof(IInvariantListAttribute),
        typeof(IInvariantListAttribute<>),
        typeof(IInvariantListAttribute<,>),
        typeof(InvariantListAttribute),
        typeof(InvariantListAttribute<>),
        typeof(InvariantListAttribute<,>)];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override TypeNode CreateNode(
        TypeCandidate candidate)
        => new XTypeNode(candidate.Symbol)
        { Syntax = candidate.Syntax, Attributes = candidate.Attributes };

    // ----------------------------------------------------

    /// <summary>
    /// The version of this generator for documentation purposes.
    /// </summary>
    public static string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    /// <summary>
    /// A string with the 'GeneratedCode' attribute of this generator for documentation purposes.
    /// </summary>
    public static string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantGenerator)}}", "{{VersionDoc}}")]
        """;
}
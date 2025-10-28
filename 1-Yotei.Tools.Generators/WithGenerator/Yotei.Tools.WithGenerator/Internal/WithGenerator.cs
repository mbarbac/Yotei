namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal partial class WithGenerator : CoreGenerator.CoreGenerator
{
#if DEBUG_WITH_GENERATOR
    /// <inheritdoc/>
    //protected override bool LaunchDebugger => true;
#endif

    // ----------------------------------------------------

    /// <summary>
    /// The version of this generator for documentation purposes.
    /// </summary>
    public static string VersionDoc => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    /// <summary>
    /// A string with the 'GeneratedCode' attribute of this generator for documentation purposes.
    /// </summary>
    public static string AttributeDoc => $$"""
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(WithGenerator)}}", "{{VersionDoc}}")]
        """;
}
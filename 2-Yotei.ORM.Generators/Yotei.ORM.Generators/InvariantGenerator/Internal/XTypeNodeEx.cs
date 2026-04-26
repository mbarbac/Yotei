namespace Yotei.ORM.Generators;

// ========================================================
public partial class XTypeNode
{
    /// <summary>
    /// Gets the version of this generator for documentation purposes.
    /// </summary>
    public static string DocVersion => Assembly.GetExecutingAssembly().GetName().Version.To3String();

    /// <summary>
    /// Gets the string that emits the attribute decoration, for documentation purposes.
    /// </summary>
    public static string DocAttribute => $$"""
        [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(InvariantGenerator)}}", "{{DocVersion}}")]
        """;

    // ----------------------------------------------------
}
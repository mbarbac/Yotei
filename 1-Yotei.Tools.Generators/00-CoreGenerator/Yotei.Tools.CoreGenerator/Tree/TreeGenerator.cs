namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents the base class for tree-oriented incremental source code generators that arrange
/// their captured elements in a hierarchical tree structure where each top node corresponds to
/// a single type, along with its child elements (if any), and its emitted in its own file.
/// <para>
/// Derived types shall be be decorated with the <see cref="GeneratorAttribute"/> attribute to be
/// recognized by the compiler. It is also expected that the <see cref="LanguageNames.CSharp"/>
/// value is used as that attribute's argument.
/// </para>
/// </summary>
internal partial class TreeGenerator : IIncrementalGenerator
{
    static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Although this method is public, it is INFRASTRUCTURE ONLY and shall not be called
    /// by application code.
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
    }
}
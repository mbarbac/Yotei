namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a property-alike syntax node captured for code generation purposes.
/// </summary>
public interface ICapturedProperty : ICaptured
{
    /// <summary>
    /// The captured type this instance logically belongs to.
    /// </summary>
    ICapturedType CapturedType { get; }

    /// <summary>
    /// The property declaration syntax captured by this instance.
    /// </summary>
    PropertyDeclarationSyntax PropertySyntax { get; }

    /// <summary>
    /// The property symbol captured by this instance.
    /// </summary>
    IPropertySymbol PropertySymbol { get; }
}
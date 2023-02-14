namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents a field-alike syntax node captured for code generation purposes.
/// </summary>
public interface ICapturedField : ICaptured
{
    /// <summary>
    /// The captured type this instance logically belongs to.
    /// </summary>
    ICapturedType CapturedType { get; }

    /// <summary>
    /// The field declaration syntax captured by this instance.
    /// </summary>
    FieldDeclarationSyntax FieldSyntax { get; }

    /// <summary>
    /// The field symbol captured by this instance.
    /// </summary>
    IFieldSymbol FieldSymbol { get; }
}
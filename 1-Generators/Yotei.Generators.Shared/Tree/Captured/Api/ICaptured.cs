namespace Yotei.Generators.Tree;

// ========================================================
/// <summary>
/// Represents an element captured by a source code generator.
/// </summary>
internal interface ICaptured
{
    /// <summary>
    /// The generator this instance refers to.
    /// </summary>
    IGenerator Generator { get; }

    /// <summary>
    /// The name of this instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The target syntax node of the captured element.
    /// </summary>
    SyntaxNode Syntax { get; }

    /// <summary>
    /// The target symbol of the captured element.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The semantic model for the captured element.
    /// </summary>
    SemanticModel SemanticModel { get; }
}
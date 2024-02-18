namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a syntax node identified for source generation.
/// </summary>
internal interface ICandidate
{
    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    SemanticModel SemanticModel { get; }

    /// <summary>
    /// The syntax node this instance wraps over.
    /// </summary>
    SyntaxNode SyntaxNode { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    ISymbol Symbol { get; }
}
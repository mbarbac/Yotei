namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a wrapper over a syntax node identified for source generation.
/// </summary>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal abstract class Candidate(SemanticModel model, SyntaxNode node, ISymbol symbol)
{
    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <summary>
    /// The syntax node this instance wraps over.
    /// </summary>
    public SyntaxNode SyntaxNode { get; } = node.ThrowWhenNull();

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public ISymbol Symbol { get; } = symbol.ThrowWhenNull();
}
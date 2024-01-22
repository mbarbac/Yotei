namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a candidate for source code generation purposes.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal abstract class Candidate(SemanticModel model, SyntaxNode syntax, ISymbol symbol)
{
    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <summary>
    /// The syntax node of this instance.
    /// </summary>
    public virtual SyntaxNode Syntax { get; } = syntax.ThrowWhenNull();

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public virtual ISymbol Symbol { get; } = symbol.ThrowWhenNull();
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="INodeCandidate"/>
internal abstract class Candidate : INodeCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public Candidate(
        ImmutableArray<AttributeData> attributes,
        SemanticModel model,
        SyntaxNode syntax,
        ISymbol symbol)
    {
        Attributes = attributes;
        SemanticModel = model;
        Syntax = syntax;
        Symbol = symbol;
    }

    /// <inheritdoc/>
    public ImmutableArray<AttributeData> Attributes { get; }

    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; }

    /// <inheritdoc/>
    public SyntaxNode Syntax { get; }

    /// <inheritdoc/>
    public ISymbol Symbol { get; }
}
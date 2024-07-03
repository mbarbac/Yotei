namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a valid candidate for source code generation.
/// </summary>
internal interface INodeCandidate : ICandidate
{
    /// <summary>
    /// The list of recognized attributes that decorates the candidate.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }

    /// <summary>
    /// The semantic model the source element is associated with.
    /// </summary>
    SemanticModel SemanticModel { get; }

    /// <summary>
    /// The syntax of the associated source element.
    /// </summary>
    SyntaxNode Syntax { get; }

    /// <summary>
    /// The symbol of the associated source element.
    /// </summary>
    ISymbol Symbol { get; }
}
namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a source code element that is a candidate for source code generation.
/// </summary>
internal interface IValidCandidate : ICandidate
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
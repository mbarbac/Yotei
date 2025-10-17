namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid candidate for source code generation.
/// </summary>
internal interface IValidCandidate : ICandidate
{
    /// <summary>
    /// The symbol of this candidate.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The syntax associated with this candidate, or null if not captured.
    /// </summary>
    SyntaxNode? Syntax { get; }

    /// <summary>
    /// The recognized attributes that decorate the candidate.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }
}
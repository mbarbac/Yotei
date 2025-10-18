namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents valid candidates for source code generation, which will be later used to build
/// the hierarchical tree-oriented nodes actually in charge of emitting source code.
/// </summary>
internal interface IValidCandidate : ICandidate
{
    /// <summary>
    /// The symbol of this candidate.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The syntax by which this candidate was captured, or null if this information is not
    /// avalilable.
    /// </summary>
    SyntaxNode? Syntax { get; }

    /// <summary>
    /// The recognized attributes that decorate the candidate.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }
}
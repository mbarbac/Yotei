namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid candidate for source code generation.
/// <br/> Candidates have not notion of source code generation hierarchy, which is only created
/// at source code emitting phase.
/// </summary>
internal interface IValidCandidate : ICandidate
{
    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if any.
    /// <br/> (Candidates may choose not to cache this data for performance purposes).
    /// </summary>
    SyntaxNode? Syntax { get; }

    /// <summary>
    /// The attributes captured for this instance, at its syntax site, or an empty one if any.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }
}
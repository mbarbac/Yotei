namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid candidate for source code generation.
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
    /// The attributes captured for this instance, or an empty array if any.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }
}
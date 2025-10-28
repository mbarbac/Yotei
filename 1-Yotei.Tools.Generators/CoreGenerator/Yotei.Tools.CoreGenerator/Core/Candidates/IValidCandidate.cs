namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid source code generation candidate and its captured information.
/// </summary>
internal interface IValidCandidate : ICandidate
{
    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if not available.
    /// </summary>
    SyntaxNode? Syntax { get; }

    /// <summary>
    /// The attributes captured for this instance, or '<c>empty</c>' if any, or if this data is
    /// not available.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }
}
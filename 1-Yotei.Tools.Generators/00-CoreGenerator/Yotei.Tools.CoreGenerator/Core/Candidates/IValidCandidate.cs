namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid source code generation candidate.
/// </summary>
internal interface IValidCandidate : ICandidate
{
    /// <summary>
    /// The symbol captured for this candidate element.
    /// </summary>
    public ISymbol Symbol { get; }
}
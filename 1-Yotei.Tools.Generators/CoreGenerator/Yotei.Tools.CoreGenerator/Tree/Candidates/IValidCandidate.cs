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
}
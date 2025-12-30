namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid candidate for source code generation. Instances of this type are just used
/// to cache the information relevant to the generator, and will be later transformed into nodes
/// attached to a given file-based hierarchy.
/// </summary>
internal interface IValidCandidate : ICandidate
{
    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    ISymbol Symbol { get; }
}
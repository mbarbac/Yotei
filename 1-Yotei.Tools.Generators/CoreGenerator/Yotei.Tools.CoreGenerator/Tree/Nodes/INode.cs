namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode : IBaseNode
{
    /// <summary>
    /// The symbol represented by this instance.
    /// </summary>
    ISymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Augments the data captured by this instance using the contents of the given element.
    /// </summary>
    /// <param name="candidate"></param>
    void Augment(ICandidate candidate);

    /// <summary>
    /// Augments the data captured by this instance using the contents of the given element.
    /// </summary>
    /// <param name="node"></param>
    void Augment(INode node);
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface ITreeNode : INode
{
    /// <summary>
    /// The tree-based hierarchy parent node of this instance.
    /// </summary>
    public INode Parent { get; }

    /// <summary>
    /// The symbol represented by this instance.
    /// </summary>
    ISymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Augments the contents of this instance with the ones obtained from the given candidate.
    /// </summary>
    /// <param name="candidate"></param>
    void Augment(IValidCandidate candidate);

    /// <summary>
    /// Augments the contents of this instance with the ones obtained from the given node.
    /// </summary>
    /// <param name="node"></param>
    void Augment(ITreeNode node);
}
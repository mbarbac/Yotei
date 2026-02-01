namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a child node in the source code generation hierarchy.
/// </summary>
internal interface IChildNode : INode
{
    /// <summary>
    /// The node that, in the source code generation hierarchy, is the parent one of this one.
    /// Note that this parent may not neccesarily be the containing syntax element.
    /// </summary>
    public INode Parent { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// candidate. This method is invoked by the hierarchy-creation process when a node for the
    /// element already exist in that hierarchy.
    /// </summary>
    /// <param name="candidate"></param>
    void Augment(ICandidate candidate);

    /// <summary>
    /// Invoked to add to the contents of this node with the information obtained from the given
    /// node. This method is invoked by the hierarchy-creation process when a node for the element
    /// already exist in that hierarchy.
    /// </summary>
    /// <param name="node"></param>
    void Augment(INode node);
}
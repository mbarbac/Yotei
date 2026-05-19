namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured tree-oriented child source code generation node that, conceptually,
/// belongs to a higher node in the generation hierarchy.
/// </summary>
public interface IChildNode : ITreeNode
{
    /// <summary>
    /// The node this instance belongs to in the source code generation hierarchy, or null if it
    /// is a detached one.
    /// <br/> If not null, then the parent node NEEDS NOT to represent the declaring element of
    /// the one carried by this instance: it is just a hierarchy generation container to organize
    /// how code is generated.
    /// </summary>
    ITreeNode? Parent { get; set; }
}
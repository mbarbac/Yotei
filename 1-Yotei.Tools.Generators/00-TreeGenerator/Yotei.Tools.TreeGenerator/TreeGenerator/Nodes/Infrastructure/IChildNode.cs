namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured tree-oriented child source code generation node.
/// <para>
/// Types that implement this interface shall implement their <see cref="IEquatable{T}"/>
/// capabilities in a generator cache friendly manner.
/// </para>
/// </summary>
public interface IChildNode : ITreeNode
{
    /// <summary>
    /// The node this instance belongs to in the source code generation hierarchy, or null if it
    /// is a detached one. If not null, then the parent node NEEDS NOT to represent the declaring
    /// element of the one carried by this instance: it is just a hierarchy artifact.
    /// </summary>
    ITreeNode? Parent { get; set; }
}
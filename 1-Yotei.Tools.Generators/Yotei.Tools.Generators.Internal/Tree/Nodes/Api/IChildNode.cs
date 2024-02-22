namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a child node in the source code generation hierarchy.
/// </summary>
internal interface IChildNode : INode
{
    /// <summary>
    /// The node in the source code generation hierarchy this instance belongs to. Note that,
    /// in some circumstances, this node might not be the same as the corresponding containing
    /// one in the original source code.
    /// </summary>
    INode ParentNode { get; }
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a child node in the source code generation hierarchy.
/// </summary>
internal interface IChildNode : INode
{
    /// <summary>
    /// The parent node to which this node belongs.
    /// <br/> In most cases it represents the syntactic container, such as the parent type or
    /// namespace, but not necessarily.
    /// </summary>
    INode ParentNode { get; }
}
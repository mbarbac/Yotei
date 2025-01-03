namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a child node in the source code generation hierarchy.
/// </summary>
internal interface IChildNode : INode
{
    /// <summary>
    /// The parent node this one belongs to, in the source code generation hierarchy.
    /// </summary>
    INode ParentNode { get; }
}
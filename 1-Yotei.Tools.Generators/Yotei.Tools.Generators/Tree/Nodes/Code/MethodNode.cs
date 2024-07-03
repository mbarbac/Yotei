namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : IChildNode
{
    /// <summary>
    /// The type-alike node this instance belongs to in the source code generation hierarchy.
    /// </summary>
    public TypeNode ParentNode { get; }
    INode IChildNode.ParentNode => ParentNode;
}
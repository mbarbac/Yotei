namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field-alike node in the source code generation hierarchy.
/// </summary>
internal class FieldNode : IChildNode
{
    /// <summary>
    /// The type-alike node this instance belongs to in the source code generation hierarchy.
    /// </summary>
    public TypeNode ParentNode { get; }
    INode IChildNode.ParentNode => ParentNode;
}
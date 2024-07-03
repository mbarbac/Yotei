namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : IChildNode
{
    /// <summary>
    /// The parent node this instance belongs to in the source code generation hierarchy, which
    /// can be either a namespace or a parent type.
    /// </summary>
    public INode ParentNode { get; }
}
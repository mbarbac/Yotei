namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a namespace-alike node in the source code generation hierarchy.
/// </summary>
internal sealed class NamespaceNode : IChildNode
{
    /// <summary>
    /// The parent node this instance belongs to in the source code generation hierarchy, which
    /// can be either a top-most file or a parent namespace.
    /// </summary>
    public INode ParentNode { get; }
}
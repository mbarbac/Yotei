namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a tree-oriented captured source code generation node.
/// </summary>
public interface ITreeNode : INode
{
    /// <summary>
    /// The c#-alike name of this element, which also serves as it unique identifier.
    /// </summary>
    string DisplayName { get; }
}
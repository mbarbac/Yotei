namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured tree-oriented source code generation node.
/// </summary>
public interface ITreeNode : INode
{
    /// <summary>
    /// The symbol represented by this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes where the associated symbol was found, or an empty list
    /// if this information is not available.
    /// </summary>
    List<SyntaxNode> SyntaxNodes { get; }

    /// <summary>
    /// The collection attributes captured for the associated symbol, or an empty list if this
    /// information is not available.
    /// </summary>
    List<AttributeData> Attributes { get; }
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface ITreeNode : INode
{
    /// <summary>
    /// The node this one belongs to in the source code generation hierarchy.
    /// </summary>
    INode ParentNode { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes where the associated symbol was found, or an empty one
    /// if this information was not captured.
    /// </summary>
    List<SyntaxNode> SyntaxNodes { get; }

    /// <summary>
    /// The collection of attributes captured at the syntax locations where the symbol was found,
    /// or an empty one if this information was not captured.
    /// </summary>
    List<AttributeData> Attributes { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to augment the information of this node with the one obtained from the given one
    /// as, for instance, adding the syntax nodes and attributes.
    /// </summary>
    /// <param name="node"></param>
    void Augment(ITreeNode node);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of this node. If it returns <see langword="false"/>, then
    /// its source code generation will be aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    bool Emit(SourceProductionContext context, CodeBuilder cb);
}
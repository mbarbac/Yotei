namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured tree oriented source code generation node.
/// </summary>
/// NOTE: Types that implement this interface must implement their equatable capabilities in
/// a incremental generator cache friendly manner.
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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to augment the information carried by this instance with the information obtained
    /// from the other one. This typically add other's syntax nodes and attributes.
    /// <br/> Inheritors typically invoke their base method first.
    /// </summary>
    /// <param name="other"></param>
    void Augment(ITreeNode other);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of this node, on the given code builder, and using the
    /// given context and options.
    /// <br/> If this method returns <see langword="false"/>, then its code generation is aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    bool Emit(ref TreeContext context, CodeBuilder cb);
}
namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured tree-oriented source code generation node.
/// <para>
/// Types that implement this interface shall implement their <see cref="IEquatable{T}"/>
/// capabilities in a generator cache friendly manner.
/// </para>
/// </summary>
public interface ITreeNode : INode
{
    /// <summary>
    /// The symbol this instance represents.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes where the associated symbol was found, or an empty list
    /// if this information is not available.
    /// </summary>
    /// Note: this list and its elements is typically not used for equality purposes, so being
    /// generator cache friendly.
    List<SyntaxNode> SyntaxNodes { get; }

    /// <summary>
    /// The collection of attributes captured for the associated symbol, or an empty list if
    /// this information is not available.
    /// </summary>
    /// Note: this list and its elements is typically not used for equality purposes, so being
    /// generator cache friendly.
    List<AttributeData> Attributes { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to augment the information captured by this instance with the information from
    /// the other given one (for instance: adding syntax nodes and attributes). Inheritors will
    /// typically invoke their base methods first.
    /// </summary>
    /// <param name="other"></param>
    void Augment(ITreeNode other);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of this node on the given code builder, using the given
    /// extended context. If it returns <see langword="false"/>, then its source code generation
    /// will be aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    bool Emit(ref TreeContext context, CodeBuilder cb);
}
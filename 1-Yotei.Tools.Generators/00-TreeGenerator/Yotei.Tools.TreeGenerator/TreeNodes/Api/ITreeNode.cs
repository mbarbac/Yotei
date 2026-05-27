namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured tree-oriented source code generation node.
/// </summary>
internal interface ITreeNode : INode
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
    /// Used to capture, at creation time, the given information if such is needed.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="attributes"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    ITreeNode With(SyntaxNode syntax, IEnumerable<AttributeData> attributes, SemanticModel model);

    /// <summary>
    /// Invoked while building the generation hierarchy to augment the information captured by
    /// this instance with the one from the other given node. This method typically captures the
    /// other syntaxes and attributes.
    /// <br/> Inheritors may want to invoke their base method first.
    /// </summary>
    /// <param name="other"></param>
    void Augment(ITreeNode other);

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to emit the source code of this node using the given context and builder.
    /// <br/> If this method returns false then the code generation of this instance is aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    bool Emit(in TreeContext context, CodeBuilder cb);
}
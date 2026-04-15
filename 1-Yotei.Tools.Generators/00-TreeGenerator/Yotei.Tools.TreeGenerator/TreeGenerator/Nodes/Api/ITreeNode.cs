namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a captured source code hierarchycal generation node.
/// </summary>
public interface ITreeNode : INode
{
    /// <summary>
    /// The parent node this instance belongs to in the source code generation hierarchy, or null
    /// if it is a detached or a top-most one. If not null, then the parent element needs NOT to
    /// represent the declaring element of the one represented by this instance.
    /// </summary>
    INode? Parent { get; }

    /// <summary>
    /// The symbol represented by this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The collection of syntax nodes captured by the generator for this instance, if any.
    /// </summary>
    List<SyntaxNode> SyntaxNodes { get; }

    /// <summary>
    /// The collection of attributes by which the element carried by this instance was found by
    /// the generator, if any.
    /// </summary>
    List<AttributeData> Attributes { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to augment the information carried by this instance with the one carried by the
    /// other given one (for instance, to add syntax nodes or attributes).
    /// </summary>
    /// <param name="node"></param>
    void Augment(ITreeNode node);

    /// <summary>
    /// Invoked to emit the source code of this node, along with the source code of its childs,
    /// if any. If this method returns <see langword="false"/>, then its source code generation
    /// will be aborted.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    bool Emit(SourceProductionContext context, CodeBuilder cb);
}
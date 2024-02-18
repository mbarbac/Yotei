namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// The node that is the hierarchical parent of this one, or null if this is a top-most one.
    /// </summary>
    INode ParentNode { get; }

    /// <summary>
    /// Invoked to validate this node, and its child ones, before source code generation.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked after validation to emit the source code for this node its child ones.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// The node this instance belongs to in the source code generation hiearchy, or null if it
    /// is an orphan one.
    /// </summary>
    INode? ParentNode { get; set; }

    /// <summary>
    /// Invoked before source code generation to validate this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked after validation to emit the source code for this node, and its child ones,
    /// if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
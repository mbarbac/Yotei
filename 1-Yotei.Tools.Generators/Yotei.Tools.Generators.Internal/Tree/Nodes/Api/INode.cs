namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// Represents the source code generation hierarchy.
    /// </summary>
    Hierarchy Hierarchy { get; }

    /// <summary>
    /// The node this instance belongs to, or null if it is a top-most one.
    /// </summary>
    INode? ParentNode { get; }

    /// <summary>
    /// Validates this node for source code generation purposes, along with its child ones,
    /// if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked after validation to emit the source code for this node, and for its child ones,
    /// if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Print(SourceProductionContext context, CodeBuilder cb);
}
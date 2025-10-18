namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// Invoked BEFORE code generation to validate this node, and all its child ones, if any.
    /// <br/> Note that this is NOT the place to capture information, at it may happen that this
    /// method is never invoked (for instance, if a parent node is invalid).
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to produce the actual source code for this node, and for all its child ones, if
    /// any, and to emit it into the given builder.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
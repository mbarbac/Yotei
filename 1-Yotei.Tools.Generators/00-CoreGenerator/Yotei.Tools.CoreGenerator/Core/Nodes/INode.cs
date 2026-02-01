namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// Invoked to validate this node, and its child ones if any, before emitting source code.
    /// Returns <see langword="false"/> if it is not a valid one and source code generation shall
    /// be aborted. Note that this method might not even be invoked if its parent node is not a
    /// valid one.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to emit the source code of this node.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
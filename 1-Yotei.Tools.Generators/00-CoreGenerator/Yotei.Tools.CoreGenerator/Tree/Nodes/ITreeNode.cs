namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a source code generation element in the hierarchy tree.
/// </summary>
internal interface ITreeNode : INode
{
    /// <summary>
    /// Invoked to validate this node, and its child ones, if any, before emitting any source code.
    /// Returns <see langword="false"/> if it is not a valid one so source code generation shall be
    /// aborted.
    /// <br/> Base methods must be called from overriding ones, unless they take full control.
    /// <br/> In addition, derived methods might not be called if the parent node (if any) is not
    /// a valid one itself.
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
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface IBaseNode
{
    /// <summary>
    /// Invoked to validate this node before source code generation. Returns '<c>false</c>' if it
    /// is not valid and source code generation shall be aborted. This method might not be invoked
    /// if its parent node is not a valid one.
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
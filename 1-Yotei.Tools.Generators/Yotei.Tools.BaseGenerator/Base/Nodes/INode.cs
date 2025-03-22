namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// Gets the compilation stored by the file node at the top of this branch hierarchy.
    /// <br/> The value of this property is obtained from the first candidate used to create this
    /// branch. Other candidates captured by the same branch are not used for this.
    /// <br/> In addition, some nodes may have a <see cref="IValidCandidate"/> property that, if
    /// not <c>null</c>, can be used to get the node's compilation from its semantic model.
    /// </summary>
    /// <returns></returns>
    Compilation GetBranchCompilation();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked before generation to validate this node, and its child ones if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to emit the source code for this node, and its child ones if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a child node in the source code generation hierarchy.
/// </summary>
internal interface IChildNode : INode
{
    /// <summary>
    /// The node in the source code generation hierarchy this instance belongs to.
    /// <br/> This node needs not to be the same as the containing element in the source code.
    /// </summary>
    INode ParentNode { get; }

    /// <summary>
    /// Invoked before source code generation to validate this node, and its child ones if any.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked after validation to emit the source code for this node, and its child ones if
    /// any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
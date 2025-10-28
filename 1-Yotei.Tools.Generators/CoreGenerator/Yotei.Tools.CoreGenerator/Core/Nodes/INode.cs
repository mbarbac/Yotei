namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode
{
    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if not available.
    /// </summary>
    SyntaxNode? Syntax { get; }

    /// <summary>
    /// The attributes captured for this instance, or '<c>empty</c>' if any, or if this data is
    /// not available.
    /// </summary>
    ImmutableArray<AttributeData> Attributes { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked BEFORE code generation to validate this node, and its child ones, if any.
    /// <br/> This method might not be invoked if a parent instance is an invalid one.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to produce the actual source code for this node, and for its child ones, if any,
    /// and to emit it into the given builder.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}
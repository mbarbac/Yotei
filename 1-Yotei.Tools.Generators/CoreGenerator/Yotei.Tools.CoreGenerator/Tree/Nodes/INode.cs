namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a node in the source code generation hierarchy.
/// </summary>
internal interface INode { }
/*
{
    /// <summary>
    /// The symbol represented by this instance.
    /// </summary>
    ISymbol Symbol { get; }

    /// <summary>
    /// The syntaxes where the element represented by this instance was found. This property
    /// can be an empty list if this information is not captured.
    /// </summary>
    CustomList<SyntaxNode> Syntaxes { get; }

    /// <summary>
    /// The attributes captured for this instance, or an empty one if any.
    /// </summary>
    CustomList<AttributeData> Attributes { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate this node before source code generation, and its child ones. Returns
    /// '<c>false</c>' if it is not valid and source code generation shall be aborted.
    /// <br/> This method might not be invoked if its parent node is not a valid one.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    bool Validate(SourceProductionContext context);

    /// <summary>
    /// Invoked to emit the source code of this node, and its child ones, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    void Emit(SourceProductionContext context, CodeBuilder cb);
}*/
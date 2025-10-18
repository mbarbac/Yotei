namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike node in source generation hierarchy.
/// </summary>
internal sealed class FieldNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public FieldNode(TypeNode parent, IFieldSymbol symbol)
    {
        ParentNode = parent;
        Symbol = symbol;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TypeNode ParentNode { get; private set => field = value.ThrowWhenNull(); }
    INode IChildNode.ParentNode => ParentNode;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IFieldSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}
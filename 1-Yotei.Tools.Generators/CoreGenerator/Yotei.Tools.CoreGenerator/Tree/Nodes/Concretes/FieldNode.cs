namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike node in the source code generation hierarchy.
/// </summary>
internal class FieldNode : FieldCandidate, INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public FieldNode(TypeNode parent, IFieldSymbol symbol) : base(symbol) => Parent = parent;

    /// <summary>
    /// The node this instance belongs to in the source code generation hierarchy.
    /// <br/> Note that its symbol needs not to be the containing one of this instance.
    /// </summary>
    public TypeNode Parent { get; init => field = value.ThrowWhenNull(); }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(
        SourceProductionContext context, CodeBuilder cb) => cb.Append($"// {this}");
}
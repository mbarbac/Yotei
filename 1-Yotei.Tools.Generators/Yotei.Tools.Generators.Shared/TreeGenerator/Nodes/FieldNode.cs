namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a field-alike node in the source code generation hierarchy.
/// </summary>
internal class FieldNode : Node
{
    /// <summary>
    /// Initializes a new instance for the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public FieldNode(TypeNode parent, FieldCandidate candidate) : base(
        parent!.Generator ??
        throw new ArgumentNullException(nameof(parent)))
    {
        Parent = parent;
        Candidate = candidate.ThrowWhenNull(nameof(candidate));
        Symbol = candidate.Symbol;
    }

    /// <summary>
    /// The candidate from which this instance was obtained, or null if it was not created in
    /// the hierarchy formation phase.
    /// </summary>
    public FieldCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance, using the given symbol.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public FieldNode(TypeNode parent, IFieldSymbol symbol) : base(
        parent!.Generator ??
        throw new ArgumentNullException(nameof(parent)))
    {
        Parent = parent;
        Symbol = symbol.ThrowWhenNull(nameof(symbol));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.ToStringEx(useSymbolType: false)}";

    /// <summary>
    /// The node in the hierarchy that contains this instance.
    /// </summary>
    public TypeNode Parent { get; }

    /// <summary>
    /// The symbol associated with this instance.
    /// </summary>
    public IFieldSymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!TypeNode.ValidateIsPartial(context, Parent.Symbol)) return false;
        if (!TypeNode.ValidateIsSupported(context, Parent.Symbol)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
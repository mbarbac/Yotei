namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a property-alike node in the source code generation hierarchy.
/// </summary>
internal partial class PropertyNode : Node
{
    /// <summary>
    /// Initializes a new instance for the given candidate.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="candidate"></param>
    public PropertyNode(TypeNode parent, PropertyCandidate candidate) : base(
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
    public PropertyCandidate? Candidate { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance, using the given symbol.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public PropertyNode(TypeNode parent, IPropertySymbol symbol) : base(
        parent!.Generator ??
        throw new ArgumentNullException(nameof(parent)))
    {
        Parent = parent;
        Symbol = symbol.ThrowWhenNull(nameof(symbol));
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Property: {Symbol.ToStringEx(useSymbolType: false)}";

    /// <summary>
    /// The node in the hierarchy that contains this instance.
    /// </summary>
    public TypeNode Parent { get; }

    /// <summary>
    /// The symbol associated with this instance.
    /// </summary>
    public IPropertySymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the contents of this instance, and its child elements, if any.
    /// Returns true if it is valid for source generation purposes, of false otherwise.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool Validate(SourceProductionContext context)
    {
        if (!Parent.Symbol.ValidateIsPartial(context)) return false;
        if (!Parent.Symbol.ValidateIsSupportedKind(context)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to append to the given code builder the contents of this instance, and its
    /// child elements, if any.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public override void Print(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
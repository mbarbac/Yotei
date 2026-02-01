namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid property-alike candidate for source code generation.
/// </summary>
internal class PropertyCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public PropertyCandidate(IPropertySymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Property: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public IPropertySymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax where this element was captured, or <see langword="null"/> if it was not captured.
    /// </summary>
    public BasePropertyDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified,  at the location where
    /// it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
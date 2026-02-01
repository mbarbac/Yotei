namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid field-alike candidate for source code generation.
/// </summary>
internal class FieldCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public FieldCandidate(IFieldSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public IFieldSymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax where this element was captured, or <see langword="null"/> if it was not captured.
    /// </summary>
    public FieldDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified,  at the location where
    /// it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
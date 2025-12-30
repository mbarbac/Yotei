namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike valid candidate for source code generation.
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

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public IPropertySymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if any.
    /// </summary>
    public BasePropertyDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The attributes by which this instance was found, or an empty collection if no information
    /// was captured.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
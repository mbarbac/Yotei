namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike source code generation candidate.
/// </summary>
internal class PropertyCandidate : ICandidate
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
    /// The symbol captured for this instance.
    /// </summary>
    public IPropertySymbol Symbol { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The syntax node captured for this instance, or <see langword="null"/> if any.
    /// </summary>
    public BasePropertyDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified, captured at the
    /// syntax location where it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike valid candidate for source code generation.
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

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public IFieldSymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if any.
    /// </summary>
    public FieldDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The attributes by which this instance was found, or an empty collection if no information
    /// was captured.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
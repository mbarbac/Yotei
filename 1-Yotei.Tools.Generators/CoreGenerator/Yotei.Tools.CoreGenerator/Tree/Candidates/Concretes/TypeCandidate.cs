namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike valid candidate for source code generation.
/// </summary>
internal class TypeCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public TypeCandidate(INamedTypeSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if this information was not
    /// captured.
    /// </summary>
    public TypeDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The attributes by which this instance was found, or an empty collection if this information
    /// was not captured.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike source code generation candidate.
/// </summary>
internal class TypeCandidate : ICandidate
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

    // ----------------------------------------------------

    /// <summary>
    /// The symbol captured for this instance.
    /// </summary>
    public INamedTypeSymbol Symbol { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The syntax node captured for this instance, or <see langword="null"/> if any.
    /// </summary>
    public BaseTypeDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified, captured at the
    /// syntax location where it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
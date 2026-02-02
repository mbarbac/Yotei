namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike source code generation candidate.
/// </summary>
internal class FieldCandidate : ICandidate
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
    /// The symbol captured for this instance.
    /// </summary>
    public IFieldSymbol Symbol { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The syntax node captured for this instance, or <see langword="null"/> if any.
    /// </summary>
    public BaseFieldDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified, captured at the
    /// syntax location where it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
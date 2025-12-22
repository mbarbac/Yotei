namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a field-alike valid candidate for source code generation.
/// <br/> Candidates have not notion of source code generation hierarchy, which is only created
/// at source code emitting phase.
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
    /// <inheritdoc cref="IValidCandidate.Syntax"/>
    /// </summary>
    public FieldDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }
    SyntaxNode? IValidCandidate.Syntax => Syntax;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ImmutableArray<AttributeData> Attributes
    {
        get;
        init => field = value.Length == 0 ? [] : (value.Any(x => x is null)
            ? throw new ArgumentException("Attributes carries null elements.").WithData(value)
            : value);
    }
    = [];
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike valid candidate for source code generation.
/// <br/> Candidates have not notion of source code generation hierarchy, which is only created
/// at source code emitting phase.
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
    /// <inheritdoc cref="IValidCandidate.Syntax"/>
    /// </summary>
    public PropertyDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }
    SyntaxNode? IValidCandidate.Syntax => Syntax;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CustomList<AttributeData> Attributes { get; } = new CustomList<AttributeData>()
    {
        ValidateElement = static (x) => x.ThrowWhenNull(),
        CompareElements = static (x, y) => x.EqualTo(y),
        IncludeDuplicate = static (_, _) => true,
    };
}
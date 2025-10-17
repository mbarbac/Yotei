namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a property-alike candidate for source code generation.
/// </summary>
public class PropertyCandidate : ICandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public PropertyCandidate(IPropertySymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Property: {Symbol.EasyName()}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICandidate.Symbol"/>
    /// </summary>
    public IPropertySymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol ICandidate.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ICandidate.Syntax"/>
    /// </summary>
    public PropertyDeclarationSyntax? Syntax { get; init => field = value; }
    SyntaxNode? ICandidate.Syntax => Syntax;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ImmutableArray<AttributeData> Attributes
    {
        get;
        init => field = value.Length == 0 ? [] : (
            value.Any(x => x is null)
            ? throw new ArgumentException("Collection of attributes carries null elements.").WithData(value)
            : value);
    } = [];
}
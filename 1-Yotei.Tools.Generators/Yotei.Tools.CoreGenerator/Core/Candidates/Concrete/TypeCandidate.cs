namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike candidate for source code generation.
/// </summary>
public class TypeCandidate : ICandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public TypeCandidate(INamedTypeSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Type: {Symbol.EasyName()}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="ICandidate.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol ICandidate.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="ICandidate.Syntax"/>
    /// </summary>
    public TypeDeclarationSyntax? Syntax { get; init => field = value; }
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
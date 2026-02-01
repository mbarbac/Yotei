namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a valid method-alike candidate for source code generation.
/// </summary>
internal class MethodCandidate : IValidCandidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    public MethodCandidate(IMethodSymbol symbol) => Symbol = symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append($"Method: {Symbol.Name}");
        sb.Append('(');
        for (int i = 0; i < Symbol.Parameters.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(Symbol.Parameters[i].Type.Name);
        }
        sb.Append(')');
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public IMethodSymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax where this element was captured, or <see langword="null"/> if it was not captured.
    /// </summary>
    public BaseMethodDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified,  at the location where
    /// it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike valid candidate for source code generation.
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
        var sb = new StringBuilder($"Method: {Symbol.Name}");

        sb.Append('('); for (int i = 0; i < Symbol.Parameters.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(Symbol.Parameters[i].Type.Name);
        }
        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public IMethodSymbol Symbol { get; init => field = value.ThrowWhenNull(); }
    ISymbol IValidCandidate.Symbol => Symbol;

    /// <summary>
    /// The syntax captured for this instance, or '<c>null</c>' if any.
    /// </summary>
    public BaseMethodDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The attributes by which this instance was found, or an empty collection if no information
    /// was captured.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
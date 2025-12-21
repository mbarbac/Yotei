namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike valid candidate for source code generation.
/// <br/> Candidates have not notion of source code generation hierarchy, which is only created
/// at source code emitting phase.
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
    /// <inheritdoc cref="IValidCandidate.Syntax"/>
    /// </summary>
    public MethodDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }
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
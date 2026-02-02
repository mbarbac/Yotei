namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike source code generation candidate.
/// </summary>
internal class MethodCandidate : ICandidate
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
    /// The symbol captured for this instance.
    /// </summary>
    public IMethodSymbol Symbol { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The syntax node captured for this instance, or <see langword="null"/> if any.
    /// </summary>
    public BaseMethodDeclarationSyntax? Syntax { get; init => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The collection of attributes by which this candidate was identified, captured at the
    /// syntax location where it was found.
    /// </summary>
    public List<AttributeData> Attributes { get; } = [];
}
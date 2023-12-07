namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a field-alike candidate for source code generation purposes.
/// </summary>
internal class FieldCandidate : Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public FieldCandidate(
        SemanticModel model, FieldDeclarationSyntax syntax, IFieldSymbol symbol)
        : base(model, syntax, symbol) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Field: {Symbol.ToStringEx(useSymbolType: true)}";

    /// <summary>
    /// The syntax node this instance is associated with.
    /// </summary>
    public new FieldDeclarationSyntax Syntax => (FieldDeclarationSyntax)base.Syntax;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public new IFieldSymbol Symbol => (IFieldSymbol)base.Symbol;
}
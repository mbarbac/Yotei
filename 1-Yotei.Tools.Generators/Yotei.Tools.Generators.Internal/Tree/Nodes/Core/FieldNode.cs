namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field in the source code generation hierarchy.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class FieldNode(
    SemanticModel model, FieldDeclarationSyntax syntax, IFieldSymbol symbol)
    : Candidate(model, syntax, symbol), INode, IFieldCandidate
{
    /// <inheritdoc/>
    public override string ToString() => $"Field: {Symbol.ContainingSymbol.Name}.{Symbol.Name}";

    /// <inheritdoc/>
    public new FieldDeclarationSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public new IFieldSymbol Symbol { get; } = symbol.ThrowWhenNull();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context)
    {
        return true;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
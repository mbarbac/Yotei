namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a property in the source code generation hierarchy.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class PropertyNode(
    SemanticModel model, PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
    : Candidate(model, syntax, symbol), INode, IPropertyCandidate
{
    /// <inheritdoc/>
    public override string ToString() => $"Property: {Symbol.ContainingSymbol.Name}.{Symbol.Name}";

    /// <inheritdoc/>
    public new PropertyDeclarationSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public new IPropertySymbol Symbol { get; } = symbol.ThrowWhenNull();

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
namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method in the source code generation hierarchy.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal class MethodNode(
    SemanticModel model, MethodDeclarationSyntax syntax, IMethodSymbol symbol)
    : Candidate(model, syntax, symbol), INode, IMethodCandidate
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var options = new EasyNameOptions(
            useGenerics: true,
            useHostType: true,
            useMemberType: true,
            useMemberArguments: true);

        return $"Method: {Symbol.EasyName(options)}";
    }

    /// <inheritdoc/>
    public new MethodDeclarationSyntax Syntax { get; } = syntax.ThrowWhenNull();

    /// <inheritdoc/>
    public new IMethodSymbol Symbol { get; } = symbol.ThrowWhenNull();

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
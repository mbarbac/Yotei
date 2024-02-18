namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a method in the source code generation hierarchy.
/// </summary>
/// <param name="parentNode"></param>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class MethodNode(
    TypeNode parentNode,
    SemanticModel model, MethodDeclarationSyntax syntaxNode, IMethodSymbol symbol)
    : Candidate(model, syntaxNode, symbol), INode, IMethodCandidate
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
    public TypeNode ParentNode { get; } = parentNode.ThrowWhenNull();
    INode INode.ParentNode => ParentNode;

    /// <inheritdoc/>
    public new MethodDeclarationSyntax Syntax { get; } = syntaxNode.ThrowWhenNull();

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
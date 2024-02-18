namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a property in the source code generation hierarchy.
/// </summary>
/// <param name="parentNode"></param>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class PropertyNode(
    TypeNode parentNode,
    SemanticModel model, PropertyDeclarationSyntax syntaxNode, IPropertySymbol symbol)
    : Candidate(model, syntaxNode, symbol), INode, IPropertyCandidate
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var options = new EasyNameOptions(
            useGenerics: true,
            useHostType: true,
            useMemberType: true,
            useMemberArguments: true);

        return $"Property: {Symbol.EasyName(options)}";
    }

    /// <inheritdoc/>
    public TypeNode ParentNode { get; } = parentNode.ThrowWhenNull();
    INode INode.ParentNode => ParentNode;

    /// <inheritdoc/>
    public new PropertyDeclarationSyntax Syntax { get; } = syntaxNode.ThrowWhenNull();

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
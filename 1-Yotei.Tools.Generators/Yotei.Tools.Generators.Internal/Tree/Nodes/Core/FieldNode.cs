namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field in the source code generation hierarchy.
/// </summary>
/// <param name="parentNode"></param>
/// <param name="model"></param>
/// <param name="syntaxNode"></param>
/// <param name="symbol"></param>
internal class FieldNode(
    TypeNode parentNode,
    SemanticModel model, FieldDeclarationSyntax syntaxNode, IFieldSymbol symbol)
    : Candidate(model, syntaxNode, symbol), INode, IFieldCandidate
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var options = new EasyNameOptions(
            useGenerics: true,
            useHostType: true,
            useMemberType: true);

        return $"Field: {Symbol.EasyName(options)}";
    }

    /// <inheritdoc/>
    public TypeNode ParentNode { get; } = parentNode.ThrowWhenNull();
    INode INode.ParentNode => ParentNode;

    /// <inheritdoc/>
    public new FieldDeclarationSyntax Syntax { get; } = syntaxNode.ThrowWhenNull();

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
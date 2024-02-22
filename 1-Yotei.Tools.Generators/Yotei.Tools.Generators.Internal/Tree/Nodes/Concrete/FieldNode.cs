namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field node in the source code generation hierarchy.
/// </summary>
/// <param name="parent"></param>
/// <param name="symbol"></param>
internal class FieldNode(TypeNode parent, IFieldSymbol symbol) : IChildNode
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var options = new EasyNameOptions(
            useGenerics: true,
            useHostType: true,
            useMemberType: true,
            useMemberArguments: true);

        return $"Field: {Symbol.EasyName(options)}";
    }

    /// <inheritdoc/>
    public TypeNode ParentNode { get; } = parent.ThrowWhenNull();
    INode IChildNode.ParentNode => ParentNode;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IFieldSymbol Symbol { get; } = symbol.ThrowWhenNull();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
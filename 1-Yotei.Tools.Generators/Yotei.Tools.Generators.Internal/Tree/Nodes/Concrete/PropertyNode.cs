namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a property node in the source code generation hierarchy.
/// </summary>
internal class PropertyNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public PropertyNode(TypeNode parent, IPropertySymbol symbol)
    {
        ParentNode = parent.ThrowWhenNull();
        Symbol = symbol.ThrowWhenNull();
    }

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
    public TypeNode ParentNode { get; }
    INode IChildNode.ParentNode => ParentNode;

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public IPropertySymbol Symbol { get; }

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
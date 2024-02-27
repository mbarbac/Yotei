namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a field node in the source code generation hierarchy.
/// </summary>
internal class FieldNode(TypeNode parent, IFieldSymbol symbol) : IChildNode
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var item = Symbol.EasyName(new EasyNameOptions(
            useGenerics: true,
            useHostType: true,
            useMemberType: true,
            useMemberArguments: true));

        return $"Field: {item}";
    }

    /// <inheritdoc cref="IChildNode.ParentNode"/>
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
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb) => cb.AppendLine($"// {this}");
}
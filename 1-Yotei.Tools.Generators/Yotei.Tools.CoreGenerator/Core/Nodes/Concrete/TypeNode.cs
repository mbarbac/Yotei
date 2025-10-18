namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike node in source generation hierarchy.
/// </summary>
internal sealed class TypeNode : IChildNode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="symbol"></param>
    public TypeNode(INode parent, INamedTypeSymbol symbol)
    {
        ParentNode = parent;
        Symbol = symbol;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Type: {Symbol.Name}";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public INode ParentNode
    {
        get;
        private set
        {
            field = value.ThrowWhenNull();
            if (field is not NamespaceNode and not TypeNode) throw new ArgumentException(
                "Parent node is not a namespace nor a type.")
                .WithData(value);
        }
    }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public INamedTypeSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}
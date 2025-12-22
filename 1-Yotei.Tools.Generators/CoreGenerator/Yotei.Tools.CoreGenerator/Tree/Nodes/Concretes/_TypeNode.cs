/*namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a type-alike node in the source code generation hierarchy.
/// </summary>
internal class TypeNode : INode
{
    /// <summary>
    /// <inheritdoc cref="INode.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol { get; }
    ISymbol INode.Symbol => Symbol;

    /// <summary>
    /// The syntaxes where the element represented by this instance was found. This property
    /// can be an empty list if this information is not captured.
    /// </summary>
    public CustomList<SyntaxNode> Syntaxes { get; } = new CustomList<SyntaxNode>()
    {
        ValidateElement = static (x) => x.ThrowWhenNull(),
        CompareElements = static (_, _) => false,
        IncludeDuplicate = static (_, _) => true,
    };
    CustomList<TypeDeclarationSyntax> Items => Syntaxes;

    /// <summary>
    /// The attributes captured for this instance, or an empty one if any.
    /// </summary>
    public CustomList<AttributeData> Attributes { get; } = new CustomList<AttributeData>()
    {
        ValidateElement = static (x) => x.ThrowWhenNull(),
        CompareElements = static (x, y) => x.EqualTo(y),
        IncludeDuplicate = static (_, _) => true,
    };

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool Validate(SourceProductionContext context) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb) => throw null;
}*/
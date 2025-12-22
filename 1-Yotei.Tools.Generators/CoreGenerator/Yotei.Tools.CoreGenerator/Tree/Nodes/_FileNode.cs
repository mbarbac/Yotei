/*namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a file, it being the top-most node in a given source code generation hierarchy.
/// </summary>
internal class FileNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="node"></param>
    [SuppressMessage("", "IDE0290")]
    public FileNode(TypeNode node) => Node = node.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString()
    {
        var symbol = Node.Symbol;
        var sb = new StringBuilder($"File: {symbol.Name}");

        if (symbol.TypeParameters.Length != 0)
        {
            sb.Append('<'); for (int i = 0; i < symbol.TypeParameters.Length; i++)
            {
                var type = symbol.TypeParameters[i];
                if (i > 0) sb.Append(", ");
                sb.Append(type.Name);
            }
            sb.Append('>');
        }
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Auxiliary instances are created by the generator to hold members found for source code
    /// generation, when their host types are not found as such.
    /// </summary>
    public bool Auxiliary { get; init; }

    /// <summary>
    /// The type node this file refers to.
    /// </summary>
    public TypeNode Node { get; }

    /// <summary>
    /// <inheritdoc cref="IValidCandidate.Symbol"/>
    /// </summary>
    public INamedTypeSymbol Symbol => Node.Symbol;
    ISymbol INode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public CustomList<TypeDeclarationSyntax> Syntaxes { get; } = new CustomList<TypeDeclarationSyntax>()
    {
        ValidateElement = static (x) => x.ThrowWhenNull(),
        CompareElements = static (_, _) => false,
        IncludeDuplicate = static (_, _) => true,
    };

    /// <summary>
    /// <inheritdoc/>
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
    public virtual bool Validate(SourceProductionContext context) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cb"></param>
    public virtual void Emit(
        SourceProductionContext context, CodeBuilder cb) => cb.Append($"// {this}");
}*/
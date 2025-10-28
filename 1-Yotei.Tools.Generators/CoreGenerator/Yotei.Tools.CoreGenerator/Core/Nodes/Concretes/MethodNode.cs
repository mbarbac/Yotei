namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a method-alike node in the source code generation hierarchy.
/// </summary>
internal class MethodNode : INode
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <<param name="parent"></param>
    /// <param name="symbol"></param>
    public MethodNode(TypeNode parent, IMethodSymbol symbol)
    {
        ParentNode = parent;
        Symbol = symbol;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder(Symbol.Name);

        if (Symbol.Parameters.Length == 0) sb.Append("()");
        else
        {
            sb.Append('(');
            for (int i = 0; i < Symbol.Parameters.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(Symbol.Parameters[i].Type.Name);
            }
            sb.Append(')');
        }
        return $"Method: {sb}";
    }

    // ----------------------------------------------------

    /// <summary>
    /// The node this instance belongs to in the source code generation hierarchy. Its symbol
    /// needs not to be the containing type.
    /// </summary>
    public TypeNode ParentNode { get; private set => field = value.ThrowWhenNull(); }

    /// <summary>
    /// The host of this instance in the source code generation hierarchy.
    /// <br/> In some scenarios, the value of this property MIGHT NOT be the containing type.
    /// </summary>
    public INamedTypeSymbol Host => ParentNode.Symbol;

    /// <summary>
    /// <inheritdoc cref="INode.Symbol"/>
    /// </summary>
    public IMethodSymbol Symbol { get; private set => field = value.ThrowWhenNull(); }
    ISymbol INode.Symbol => Symbol;

    /// <summary>
    /// <inheritdoc cref="INode.Syntax"/>
    /// </summary>
    public MethodDeclarationSyntax? Syntax { get; init; }
    SyntaxNode? INode.Syntax => Syntax;

    /// <summary>
    /// The attributes captured for this instance, or '<c>empty</c>' if any, or if this data is
    /// not available.
    /// </summary>
    public ImmutableArray<AttributeData> Attributes
    {
        get;
        init => field = value.Length == 0 ? [] : (value.Any(x => x is null)
            ? throw new ArgumentException("Collection of attributes carries null elements.").WithData(value)
            : value);
    } = [];

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
    public virtual void Emit(SourceProductionContext context, CodeBuilder cb)
    {
        cb.AppendLine($"// {this}");
    }
}
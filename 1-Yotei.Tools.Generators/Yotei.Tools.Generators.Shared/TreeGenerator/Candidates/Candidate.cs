namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a candidate for source code generation purposes.
/// </summary>
/// <param name="model"></param>
/// <param name="syntax"></param>
/// <param name="symbol"></param>
internal abstract class Candidate(SemanticModel model, SyntaxNode syntax, ISymbol symbol)
{
    /// <summary>
    /// Allow asking semantic questions about a tree of syntax nodes in a compilation.
    /// </summary>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <summary>
    /// The syntax node of this instance.
    /// </summary>
    public SyntaxNode Syntax { get; } = syntax.ThrowWhenNull();

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public ISymbol Symbol { get; } = symbol.ThrowWhenNull();

    // ----------------------------------------------------

    /// <summary>
    /// Returns the file name, without extensions, where this candidate will emit its code.
    /// </summary>
    /// <returns></returns>
    public abstract string GetFileName();

    /// <summary>
    /// Returns a file name based upon the name of the tail-most type.
    /// </summary>
    /// <returns></returns>
    protected string GetTypeFileName()
    {
        var parts = new List<string>();

        foreach (var ns in NamespaceSyntaxChain)
        {
            var name = ns.Name.LongName();
            var items = name.Split('.');
            parts.AddRange(items);
        }
        foreach (var tp in TypeSyntaxChain)
        {
            var name = tp.Identifier.Text;
            var items = name.Split('.');
            parts.AddRange(items);
        }
        parts.Reverse();

        return string.Join(".", parts);
    }

    // ----------------------------------------------------

    ImmutableArray<BaseNamespaceDeclarationSyntax>? _NamespaceSyntaxChain = null;
    ImmutableArray<TypeDeclarationSyntax>? _TypeSyntaxChain = null;
    ImmutableArray<INamedTypeSymbol>? _TypeSymbolChain = null;

    /// <summary>
    /// The chain of namespace syntax declarations to this candidate.
    /// </summary>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain
        => _NamespaceSyntaxChain ??= Syntax.GetNamespaceSyntaxChain();

    /// <summary>
    /// The chain of type syntax declarations to this candidate, including itself if it is a
    /// type-alike one.
    /// </summary>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain
        => _TypeSyntaxChain ??= Syntax.GetTypeSyntaxChain();

    /// <summary>
    /// The chain of type symbols to this candidate, including itself if it is a type-alike one.
    /// </summary>
    public ImmutableArray<INamedTypeSymbol> TypeSymbolChain
        => _TypeSymbolChain ??= Symbol.GetTypeSymbolChain();
}
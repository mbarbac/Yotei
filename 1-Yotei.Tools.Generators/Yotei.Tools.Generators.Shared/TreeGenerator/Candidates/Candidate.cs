namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a source code generation candidate identified and validated by a tree-oriented
/// generator.
/// </summary>
internal abstract class Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    public Candidate(SemanticModel semanticModel, SyntaxNode syntax, ISymbol symbol)
    {
        SemanticModel = semanticModel.ThrowWhenNull(nameof(semanticModel));
        Syntax = syntax.ThrowWhenNull(nameof(syntax));
        Symbol = symbol.ThrowWhenNull(nameof(symbol));
    }

    /// <summary>
    /// The semantic model this instance is associated with.
    /// </summary>
    public SemanticModel SemanticModel { get; }

    /// <summary>
    /// The syntax node this instance is associated with.
    /// </summary>
    public SyntaxNode Syntax { get; }

    /// <summary>
    /// The symbol this instance is associated with.
    /// </summary>
    public ISymbol Symbol { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The chain of namespace syntax declarations to this candidate.
    /// </summary>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain => _NamespaceSyntaxChain ??= Syntax.GetNamespaceSyntaxChain();
    ImmutableArray<BaseNamespaceDeclarationSyntax>? _NamespaceSyntaxChain = null;

    /// <summary>
    /// The chain of type syntax declarations to this candidate, including itself if it is a
    /// type-alike one.
    /// </summary>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain => _TypeSyntaxChain ??= Syntax.GetTypeSyntaxChain();
    ImmutableArray<TypeDeclarationSyntax>? _TypeSyntaxChain = null;

    /// <summary>
    /// The chain of type symbols to this candidate, including itself if it is a type-alike one.
    /// </summary>
    public ImmutableArray<ITypeSymbol> TypeSymbolChain => _TypeSymbolChain ??= Symbol.GetTypeSymbolChain();
    ImmutableArray<ITypeSymbol>? _TypeSymbolChain = null;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the file name of the top-most namespace associated with this candidate.
    /// </summary>
    /// <returns></returns>
    public string GetTopNamespaceFile()
    {
        var name = NamespaceSyntaxChain[0].Name.LongName();
        var parts = name.Split('.');

        Array.Reverse(parts);
        return string.Join(".", parts);
    }

    /// <summary>
    /// Returns the file name of the tail type associated with this candidate.
    /// </summary>
    /// <returns></returns>
    public string GetTailTypeFile()
    {
        var parts = new List<string>();

        foreach (var ns in NamespaceSyntaxChain)
        {
            var name = ns.Name.LongName();
            parts.Add(name);
        }
        foreach (var tp in TypeSyntaxChain)
        {
            var name = tp.Identifier.Text;
            parts.Add(name);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }
}
namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a source code generation candidate identified by a given generator.
/// </summary>
internal abstract class Candidate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="syntax"></param>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public Candidate(SemanticModel model, SyntaxNode syntax, ISymbol symbol)
    {
        SemanticModel = model;
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

    ImmutableArray<BaseNamespaceDeclarationSyntax>? _NamespaceSyntaxChain = null;
    ImmutableArray<TypeDeclarationSyntax>? _TypeSyntaxChain = null;
    ImmutableArray<ITypeSymbol>? _TypeSymbolChain = null;

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
    public ImmutableArray<ITypeSymbol> TypeSymbolChain
        => _TypeSymbolChain ??= Symbol.GetTypeSymbolChain();

    // ----------------------------------------------------

    /// <summary>
    /// Returns the file name, without extensions, where this candidate emits its source code.
    /// </summary>
    /// <returns></returns>
    public virtual string GetFileName()
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
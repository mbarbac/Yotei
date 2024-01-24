namespace Yotei.Tools.Generators.Internal;

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
    /// Returns the file name where this candidate will emit its code, without extensions.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// The default implementation of this method returns a name that starts with the tail-most
    /// type followed by its inheritance and namespace chains, each element separated with
    /// dots.
    /// </remarks>
    public virtual string GetFileName()
    {
        List<string> parts = [];

        foreach (var ns in NamespaceSyntaxChain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        var options = new EasyNameOptions(
            fullTypeName: false,
            typeParameters: true,
            nullableAnnotation: false);

        foreach (var tp in TypeSymbolChain)
        {
            var name = tp.EasyName(options);
            name = name.Replace('<', '[');
            name = name.Replace('>', ']');
            name = name.RemoveAll(' ');
            parts.Add(name);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }
}
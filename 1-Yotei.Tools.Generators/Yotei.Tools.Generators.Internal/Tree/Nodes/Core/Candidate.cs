namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <inheritdoc cref="ICandidate"/>
/// <param name="model"></param>
/// <param name="node"></param>
/// <param name="symbol"></param>
internal abstract class Candidate(SemanticModel model, SyntaxNode node, ISymbol symbol)
    : ICandidate
{
    /// <inheritdoc/>
    public SemanticModel SemanticModel { get; } = model.ThrowWhenNull();

    /// <inheritdoc/>
    public SyntaxNode Syntax { get; } = node.ThrowWhenNull();

    /// <inheritdoc/>
    public ISymbol Symbol { get; } = symbol.ThrowWhenNull();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual string GetFileName() => GetFileNameByNamespace();

    /// <summary>
    /// Gets the name of the file where this candidate will emit its source code using its
    /// tail-most namespace.
    /// </summary>
    /// <returns></returns>
    public string GetFileNameByNamespace()
    {
        List<string> parts = [];

        foreach (var ns in NamespaceSyntaxChain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }

    /// <summary>
    /// Gets the name of the file where this candidate will emit its source code using its
    /// top-most type.
    /// </summary>
    /// <returns></returns>
    public string GetFileNameByType()
    {
        List<string> parts = [];

        foreach (var ns in NamespaceSyntaxChain)
        {
            var name = ns.Name.LongName();
            var temps = name.Split('.');
            parts.AddRange(temps);
        }

        var options = new EasyNameOptions(useGenerics: true);
        foreach (var tp in TypeSymbolChain)
        {
            var name = tp.EasyName(options);
            name = name.Replace('<', '[');
            name = name.Replace('>', ']');
            name = name.RemoveAll('?');
            name = name.RemoveAll(' ');
            parts.Add(name);
        }

        parts.Reverse();
        return string.Join(".", parts);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ImmutableArray<BaseNamespaceDeclarationSyntax> NamespaceSyntaxChain => _NamespaceSyntaxChain ??= Syntax.GetNamespaceSyntaxChain();
    ImmutableArray<BaseNamespaceDeclarationSyntax>? _NamespaceSyntaxChain;

    /// <inheritdoc/>
    public ImmutableArray<TypeDeclarationSyntax> TypeSyntaxChain => _TypeSyntaxChain ??= Syntax.GetTypeSyntaxChain();
    ImmutableArray<TypeDeclarationSyntax>? _TypeSyntaxChain;

    /// <inheritdoc/>
    public ImmutableArray<INamedTypeSymbol> TypeSymbolChain => _TypeSymbolChain ??= Symbol.GetTypeSymbolChain();
    ImmutableArray<INamedTypeSymbol>? _TypeSymbolChain;
}
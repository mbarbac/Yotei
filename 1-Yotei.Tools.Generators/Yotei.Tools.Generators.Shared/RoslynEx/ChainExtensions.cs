namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class ChainExtensions
{
    /// <summary>
    /// Returns the chain of namespace declaration syntaxes to the given node, including itself
    /// if it was a namespace declaration.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static ImmutableArray<BaseNamespaceDeclarationSyntax> GetNamespaceSyntaxChain(
        this SyntaxNode node)
    {
        List<BaseNamespaceDeclarationSyntax> list = [];
        SyntaxNode? temp = node;

        while (temp != null)
        {
            switch (temp)
            {
                case NamespaceDeclarationSyntax item: list.Add(item); break;
                case FileScopedNamespaceDeclarationSyntax item: list.Add(item); break;
            }
            temp = temp.Parent;
        }

        list.Reverse();
        return list.ToImmutableArray();
    }

    /// <summary>
    /// Returns the chain of type declaration syntaxes to the given node, including itself if it
    /// was a type declaration.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static ImmutableArray<TypeDeclarationSyntax> GetTypeSyntaxChain(
        this SyntaxNode node)
    {
        List<TypeDeclarationSyntax> list = [];
        SyntaxNode? temp = node;

        while (temp != null)
        {
            if (temp is TypeDeclarationSyntax type) list.Add(type);
            temp = temp.Parent;
        }

        list.Reverse();
        return list.ToImmutableArray();
    }

    /// <summary>
    /// Returns the chain of type symbols to the given one, including itself if it was a type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ImmutableArray<ITypeSymbol> GetTypeSymbolChain(
        this ISymbol symbol)
    {
        List<ITypeSymbol> list = [];
        ISymbol? temp = symbol;

        while (temp != null)
        {
            if (temp is ITypeSymbol type && type.IsType) list.Add(type);
            temp = temp.ContainingType;
        }

        list.Reverse();
        return list.ToImmutableArray();
    }
}
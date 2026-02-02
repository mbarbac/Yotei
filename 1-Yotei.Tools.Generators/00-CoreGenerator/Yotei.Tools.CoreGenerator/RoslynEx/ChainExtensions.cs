namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ChainExtensions
{
    /// <summary>
    /// Gets the chain of elements up to the given one, included. The <paramref name="accept"/>
    /// delegate determines if a given element shall be used or not. If <see langword="null"/>,
    /// then default criteria is used.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="accept"></param>
    /// <returns></returns>
    public static List<ISymbol> GetChain(
        this ISymbol item, Func<ISymbol, bool>? accept = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        accept ??= AllowedChainSymbol;

        List<ISymbol> list = []; while (item is not null)
        {
            if (accept(item)) list.Add(item);
            item = item.ContainingSymbol;
        }
        list.Reverse();
        return list;
    }

    public static bool AllowedChainSymbol(ISymbol item) => item switch
    {
        INamespaceSymbol temp => !temp.IsGlobalNamespace,
        INamedTypeSymbol => true,
        IPropertySymbol => true,
        IFieldSymbol => true,
        IMethodSymbol => true,
        _ => false
    };

    /// <summary>
    /// Gets the chain of elements up to the given one, included.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static List<SyntaxNode> GetChain(
        this SyntaxNode item, Func<SyntaxNode, bool>? accept = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        accept ??= AllowedChainSyntax;

        List<SyntaxNode> list = []; while (item is not null)
        {
            if (accept(item)) list.Add(item);
            item = item.Parent;
        }        
        list.Reverse();
        return list;
    }

    public static bool AllowedChainSyntax(SyntaxNode item) => item switch
    {
        CompilationUnitSyntax => true,
        BaseNamespaceDeclarationSyntax => true,
        BaseTypeDeclarationSyntax => true,
        BasePropertyDeclarationSyntax => true,
        BaseFieldDeclarationSyntax => true,
        BaseMethodDeclarationSyntax => true,
        _ => false
    };
}
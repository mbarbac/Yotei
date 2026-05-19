namespace Yotei.Tools.Generators;

// ========================================================
public static class ChainExtensions
{
    /// <summary>
    /// Gets the chain of symbols up to the given one, such as combined describes the declaring
    /// elements.
    /// <br/> The given symbol is NOT INCLUDED in the returned list.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<ISymbol> GetChain(this ISymbol symbol)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        List<ISymbol> list = [];
        while ((symbol = symbol!.ContainingSymbol) is not null)
        {
            var valid = symbol switch
            {
                INamespaceSymbol temp => !temp.IsGlobalNamespace,
                INamedTypeSymbol => true,
                IPropertySymbol => true,
                IFieldSymbol => true,
                IMethodSymbol => true,
                IEventSymbol => true,
                _ => false,
            };
            if (valid) list.Insert(0, symbol);
        }
        return list;
    }

    /// <summary>
    /// Gets the chain of symbols up to the given one, such as combined describes the declaring
    /// elements.
    /// <br/> The given syntax node is NOT INCLUDED in the returned list.
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static List<SyntaxNode> GetChain(this SyntaxNode syntax)
    {
        ArgumentNullException.ThrowIfNull(syntax);

        List<SyntaxNode> list = [];
        while ((syntax = syntax!.Parent!) is not null)
        {
            var valid = syntax switch
            {
                CompilationUnitSyntax => true,
                BaseNamespaceDeclarationSyntax => true,
                BaseTypeDeclarationSyntax => true,
                BasePropertyDeclarationSyntax => true,
                BaseFieldDeclarationSyntax => true,
                BaseMethodDeclarationSyntax => true,
                _ => false,
            };
            if (valid) list.Insert(0, syntax);
        }
        return list;
    }
}
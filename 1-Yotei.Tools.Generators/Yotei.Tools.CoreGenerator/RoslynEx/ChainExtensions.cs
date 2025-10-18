namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ChainExtensions
{
    /// <summary>
    /// Gets the chain of C#-alike syntax elements up to the given one, included.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static List<SyntaxNode> GetChain(this SyntaxNode syntax)
    {
        List<SyntaxNode> list = [syntax];

        while ((syntax = syntax!.Parent!) is not null)
        {
            switch (syntax)
            {
                case NamespaceDeclarationSyntax item: list.Add(item); break;
                case FileScopedNamespaceDeclarationSyntax item: list.Add(item); break;
                case TypeDeclarationSyntax item: list.Add(item); break;
            }
        }

        list.Reverse();
        return list;
    }

    /// <summary>
    /// Gets the chain of C#-alike symbol elements up to the given one, included.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<ISymbol> GetChain(this ISymbol symbol)
    {
        List<ISymbol> list = [symbol];

        while ((symbol = symbol!.ContainingSymbol) is not null)
        {
            switch (symbol)
            {
                case INamespaceSymbol item: list.Add(item); break;
                case INamedTypeSymbol item: list.Add(item); break;
            }
        }

        list.Reverse();
        return list;
    }
}
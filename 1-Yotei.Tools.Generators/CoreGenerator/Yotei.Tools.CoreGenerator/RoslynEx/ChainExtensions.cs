namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ChainExtensions
{
    /// <summary>
    /// Gets the chain of elements up to the given one, included.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static List<ISymbol> GetChain(this ISymbol item)
    {
        item.ThrowWhenNull();

        List<ISymbol> list = [item];
        while ((item = item.ContainingSymbol) != null)
        {
            switch (item)
            {
                case INamespaceSymbol temp: if (!temp.IsGlobalNamespace) list.Add(temp); break;
                case INamedTypeSymbol temp: list.Add(temp); break;
                case IPropertySymbol temp: list.Add(temp); break;
                case IFieldSymbol temp: list.Add(temp); break;
                case IMethodSymbol temp: list.Add(temp); break;
            }
        }
        list.Reverse();
        return list;
    }

    /// <summary>
    /// Gets the chain of elements up to the given one, included.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static List<SyntaxNode> GetChain(this SyntaxNode item)
    {
        item.ThrowWhenNull();

        List<SyntaxNode> list = [item];
        while ((item = item.Parent!) != null)
        {
            switch (item)
            {
                case CompilationUnitSyntax temp: list.Add(temp); break;
                case BaseNamespaceDeclarationSyntax temp: list.Add(temp); break;
                case TypeDeclarationSyntax temp: list.Add(temp); break;
                case PropertyDeclarationSyntax temp: list.Add(temp); break;
                case FieldDeclarationSyntax temp: list.Add(temp); break;
                case MethodDeclarationSyntax temp: list.Add(temp); break;
            }
        }
        list.Reverse();
        return list;
    }
}
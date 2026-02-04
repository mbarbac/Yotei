namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class ChainExtensions
{
    /// <summary>
    /// Gets the chain of elements up to the given one, included. The given predicate determines
    /// the kind of the elements to be included in the chain. If <see langword="null"/>, then a
    /// default criteria is used.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<ISymbol> GetElementChain(
        this ISymbol item,
        Func<ISymbol, bool>? predicate = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        predicate ??= IsChainSymbolAllowed;

        List<ISymbol> list = []; while (item is not null)
        {
            if (predicate(item)) list.Add(item);
            item = item.ContainingSymbol;
        }
        list.Reverse();
        return list;
    }

    public static bool IsChainSymbolAllowed(ISymbol item) => item switch
    {
        INamespaceSymbol temp => !temp.IsGlobalNamespace,
        INamedTypeSymbol => true,
        IPropertySymbol => true,
        IFieldSymbol => true,
        IMethodSymbol => true,
        IEventSymbol => true,
        _ => false
    };

    /// <summary>
    /// Gets the chain of elements up to the given one, included. The given predicate determines
    /// the kind of the elements to be included in the chain. If <see langword="null"/>, then a
    /// default criteria is used.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<SyntaxNode> GetElementChain(
        this SyntaxNode item,
        Func<SyntaxNode, bool>? predicate = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        predicate ??= IsChainSyntaxAllowed;

        List<SyntaxNode> list = []; while (item is not null)
        {
            if (predicate(item)) list.Add(item);
            item = item.Parent;
        }
        list.Reverse();
        return list;
    }

    public static bool IsChainSyntaxAllowed(SyntaxNode item) => item switch
    {
        CompilationUnitSyntax => true,
        BaseNamespaceDeclarationSyntax => true,
        BaseTypeDeclarationSyntax => true,
        BasePropertyDeclarationSyntax => true, // Includes events
        BaseFieldDeclarationSyntax => true,
        BaseMethodDeclarationSyntax => true,
        _ => false
    };
}
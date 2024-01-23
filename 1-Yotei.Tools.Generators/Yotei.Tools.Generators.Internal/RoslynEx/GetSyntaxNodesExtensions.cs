namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class GetSyntaxNodesExtensions
{
    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. The returned
    /// collection might be an empty one if there are no references to that symbol in the code
    /// being compiled, as for instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ImmutableArray<BaseTypeDeclarationSyntax> GetSyntaxNodes(this ITypeSymbol symbol)
    {
        symbol.ThrowWhenNull();

        var list = new CustomList<BaseTypeDeclarationSyntax>()
        {
            Comparer = (x, y) => x.IsEquivalentTo(y),
            CanInclude = (@this, item) => @this.IndexOf(item) < 0,
    };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
            if (dec.GetSyntax() is BaseTypeDeclarationSyntax temp) list.Add(temp);

        return list.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. The returned
    /// collection might be an empty one if there are no references to that symbol in the code
    /// being compiled, as for instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ImmutableArray<PropertyDeclarationSyntax> GetSyntaxNodes(this IPropertySymbol symbol)
    {
        symbol.ThrowWhenNull();

        var list = new CustomList<PropertyDeclarationSyntax>()
        {
            Comparer = (x, y) => x.IsEquivalentTo(y),
            CanInclude = (@this, item) => @this.IndexOf(item) < 0,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
            if (dec.GetSyntax() is PropertyDeclarationSyntax temp) list.Add(temp);

        return list.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. The returned
    /// collection might be an empty one if there are no references to that symbol in the code
    /// being compiled, as for instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ImmutableArray<FieldDeclarationSyntax> GetSyntaxNodes(this IFieldSymbol symbol)
    {
        symbol.ThrowWhenNull();

        var list = new CustomList<FieldDeclarationSyntax>()
        {
            Comparer = (x, y) => x.IsEquivalentTo(y),
            CanInclude = (@this, item) => @this.IndexOf(item) < 0,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var vartor = dec.GetSyntax() as VariableDeclaratorSyntax;
            var vartion = vartor?.Parent as VariableDeclarationSyntax;
            if (vartion?.Parent is FieldDeclarationSyntax temp) list.Add(temp);
        }

        return list.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. The returned
    /// collection might be an empty one if there are no references to that symbol in the code
    /// being compiled, as for instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ImmutableArray<MethodDeclarationSyntax> GetSyntaxNodes(this IMethodSymbol symbol)
    {
        symbol.ThrowWhenNull();

        var list = new CustomList<MethodDeclarationSyntax>()
        {
            Comparer = (x, y) => x.IsEquivalentTo(y),
            CanInclude = (@this, item) => @this.IndexOf(item) < 0,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
            if (dec.GetSyntax() is MethodDeclarationSyntax temp) list.Add(temp);

        return list.ToImmutableArray();
    }
}
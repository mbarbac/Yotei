namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class SyntaxNodeExtensions
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

        var list = new List<BaseTypeDeclarationSyntax>();

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is BaseTypeDeclarationSyntax item &&
                list.Find(x => ReferenceEquals(x, item)) == null)
                list.Add(item);
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
    public static ImmutableArray<PropertyDeclarationSyntax> GetSyntaxNodes(this IPropertySymbol symbol)
    {
        symbol.ThrowWhenNull();

        var list = new List<PropertyDeclarationSyntax>();

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is PropertyDeclarationSyntax item &&
                list.Find(x => ReferenceEquals(x, item)) == null)
                list.Add(item);
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
    public static ImmutableArray<FieldDeclarationSyntax> GetSyntaxNodes(this IFieldSymbol symbol)
    {
        symbol.ThrowWhenNull();

        var list = new List<FieldDeclarationSyntax>();

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is FieldDeclarationSyntax item &&
                list.Find(x => ReferenceEquals(x, item)) == null)
                list.Add(item);
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

        var list = new List<MethodDeclarationSyntax>();

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is MethodDeclarationSyntax item &&
                list.Find(x => ReferenceEquals(x, item)) == null)
                list.Add(item);
        }
        return list.ToImmutableArray();
    }
}
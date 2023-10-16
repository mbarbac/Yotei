namespace Yotei.Tools.Generators;

// ========================================================
internal static class MemberSymbolExtensions
{
    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. This list can be
    /// an empty one if there are no references to that symbol in the code being compiled, as for
    /// instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<BaseTypeDeclarationSyntax> GetSyntaxNodes(this ITypeSymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<BaseTypeDeclarationSyntax>()
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AcceptDuplicate = (item) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var temp = dec.GetSyntax() as BaseTypeDeclarationSyntax;
            if (temp != null) list.Add(temp);
        }

        return list.ToList();
    }

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. This list can be
    /// an empty one if there are no references to that symbol in the code being compiled, as for
    /// instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<PropertyDeclarationSyntax> GetSyntaxNodes(this IPropertySymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<PropertyDeclarationSyntax>
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AcceptDuplicate = (item) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var temp = dec.GetSyntax() as PropertyDeclarationSyntax;
            if (temp != null) list.Add(temp);
        }

        return list.ToList();
    }

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. This list can be
    /// an empty one if there are no references to that symbol in the code being compiled, as for
    /// instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<FieldDeclarationSyntax> GetSyntaxNodes(this IFieldSymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<FieldDeclarationSyntax>
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AcceptDuplicate = (item) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var vartor = dec.GetSyntax() as VariableDeclaratorSyntax;
            var vartion = vartor?.Parent as VariableDeclarationSyntax;
            var temp = vartion?.Parent as FieldDeclarationSyntax;
            if (temp != null) list.Add(temp);
        }

        return list.ToList();
    }

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. This list can be
    /// an empty one if there are no references to that symbol in the code being compiled, as for
    /// instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<MethodDeclarationSyntax> GetSyntaxNodes(this IMethodSymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<MethodDeclarationSyntax>
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AcceptDuplicate = (item) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var temp = dec.GetSyntax() as MethodDeclarationSyntax;
            if (temp != null) list.Add(temp);
        }

        return list.ToList();
    }
}
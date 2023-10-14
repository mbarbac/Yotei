namespace Yotei.Tools.Generators.Shared;

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
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var list = new NoDuplicatesList<BaseTypeDeclarationSyntax>
        {
            Equivalent = (x, y) => x.IsEquivalentTo(y),
            ThrowDuplicates = false
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
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var list = new NoDuplicatesList<PropertyDeclarationSyntax>
        {
            Equivalent = (x, y) => x.IsEquivalentTo(y),
            ThrowDuplicates = false
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
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var list = new NoDuplicatesList<FieldDeclarationSyntax>
        {
            Equivalent = (x, y) => x.IsEquivalentTo(y),
            ThrowDuplicates = false
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
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        var list = new NoDuplicatesList<MethodDeclarationSyntax>
        {
            Equivalent = (x, y) => x.IsEquivalentTo(y),
            ThrowDuplicates = false
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var temp = dec.GetSyntax() as MethodDeclarationSyntax;
            if (temp != null) list.Add(temp);
        }

        return list.ToList();
    }
}
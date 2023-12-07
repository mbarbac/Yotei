namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class SymbolExtensions
{
    /// <summary>
    /// Gets the collection of attributes whose name match the given one, applied to the given
    /// symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attrName"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, string attrName)
    {
        return symbol
            .GetAttributes()
            .Where(x => x.AttributeClass != null && x.AttributeClass.Name == attrName);
    }

    /// <summary>
    /// Determines if the given symbol has any attribute with the given name applied to it.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attrName"></param>
    /// <returns></returns>
    public static bool HasAttributes(
        this ISymbol symbol, string attrName) => symbol.GetAttributes(attrName).Any();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of syntax nodes where the given symbol is declared. This list can be
    /// an empty one if there are no references to that symbol in the code being compiled, as for
    /// instance when it is defined in an external assembly.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [SuppressMessage("", "IDE0305")]
    public static List<BaseTypeDeclarationSyntax> GetSyntaxNodes(this ITypeSymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<BaseTypeDeclarationSyntax>()
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AllowDuplicate = (x, y) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is BaseTypeDeclarationSyntax temp) list.Add(temp);
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
    [SuppressMessage("", "IDE0305")]
    public static List<PropertyDeclarationSyntax> GetSyntaxNodes(this IPropertySymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<PropertyDeclarationSyntax>
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AllowDuplicate = (x, y) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is PropertyDeclarationSyntax temp) list.Add(temp);
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
    [SuppressMessage("", "IDE0305")]
    public static List<FieldDeclarationSyntax> GetSyntaxNodes(this IFieldSymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<FieldDeclarationSyntax>
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AllowDuplicate = (x, y) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            var vartor = dec.GetSyntax() as VariableDeclaratorSyntax;
            var vartion = vartor?.Parent as VariableDeclarationSyntax;
            if (vartion?.Parent is FieldDeclarationSyntax temp) list.Add(temp);
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
    [SuppressMessage("", "IDE0305")]
    public static List<MethodDeclarationSyntax> GetSyntaxNodes(this IMethodSymbol symbol)
    {
        symbol.ThrowWhenNull(nameof(symbol));

        var list = new CoreList<MethodDeclarationSyntax>
        {
            Compare = (x, y) => x.IsEquivalentTo(y),
            AllowDuplicate = (x, y) => false,
        };

        foreach (var dec in symbol.DeclaringSyntaxReferences)
        {
            if (dec.GetSyntax() is MethodDeclarationSyntax temp) list.Add(temp);
        }

        return list.ToList();
    }
}
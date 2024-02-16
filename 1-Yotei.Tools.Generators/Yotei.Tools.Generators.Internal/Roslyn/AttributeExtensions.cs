namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns a flattened list with the collection of attributes in a given syntax list.
    /// </summary>
    /// <param name="sources"></param>
    /// <returns></returns>
    public static List<AttributeSyntax> GetAttributes(this SyntaxList<AttributeListSyntax> sources)
    {
        var list = new List<AttributeSyntax>();

        foreach (var source in sources)
            foreach (var attr in source.Attributes)
                list.Add(attr);

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of attributes applied to the given symbol, whose names match the
    /// given one
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
}
namespace Yotei.Tools.Generators;

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
        symbol = symbol.ThrowWhenNull(nameof(symbol));
        attrName = attrName.NotNullNotEmpty(nameof(attrName));

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
    public static bool HasAttributes(this ISymbol symbol, string attrName)
    {
        return symbol.GetAttributes(attrName).Any();
    }
}
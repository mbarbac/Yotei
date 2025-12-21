namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns the collection of attributes decorating the given symbol whose classes match the
    /// given yype.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();
        return symbol.GetAttributes().Where(x => x.Match(type));
    }
}
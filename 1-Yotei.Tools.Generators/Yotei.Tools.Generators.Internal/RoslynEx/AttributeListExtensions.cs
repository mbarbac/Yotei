namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class AttributeListExtensions
{
    /// <summary>
    /// Returns a flattened collection with the attributes in the given syntax list.
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
}
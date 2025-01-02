namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns a flattened list with the attributes found in the given collection of sources.
    /// No attempts are made to prevent any kind of duplications.
    /// </summary>
    /// <param name="sources"></param>
    /// <returns></returns>
    public static List<AttributeSyntax> GetAttributes(this SyntaxList<AttributeListSyntax> sources)
    {
        return sources.SelectMany(x => x.Attributes).ToList();
    }
}
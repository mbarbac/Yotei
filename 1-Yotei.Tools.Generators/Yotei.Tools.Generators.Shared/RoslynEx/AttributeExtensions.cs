namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns the value of the named argument whose name is given of the given attribute, or
    /// null if any is found.
    /// </summary>
    /// <param name="attrData"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypedConstant? GetNamedArgument(this AttributeData attrData, string name)
    {
        foreach (var item in attrData.NamedArguments) if (item.Key == name) return item.Value;
        return null;
    }
}
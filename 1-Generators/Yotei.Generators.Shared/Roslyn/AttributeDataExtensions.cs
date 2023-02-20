namespace Yotei.Generators;

// ========================================================
internal static class AttributeDataExtensions
{
    /// <summary>
    /// Returns the typed constant data of the argument whose name is given of the given
    /// attribute, or null if any is found.
    /// </summary>
    /// <param name="attrData"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypedConstant? GetNamedArgument(this AttributeData attrData, string name)
    {
        attrData = attrData.ThrowIfNull(nameof(attrData));
        name = name.NotNullNotEmpty(nameof(name));

        foreach (var item in attrData.NamedArguments) if (item.Key == name) return item.Value;
        return null;
    }
}
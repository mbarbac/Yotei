namespace Yotei.Tools.Generators;

// ========================================================
internal static class AttributeDataExtensions
{
    /// <summary>
    /// Returns the value of the named argument in the given attribute whose name is given, or
    /// null if any is found.
    /// </summary>
    /// <param name="attrData"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypedConstant? GetNamedArgument(this AttributeData attrData, string name)
    {
        attrData = attrData.ThrowWhenNull(nameof(attrData));
        name = name.NotNullNotEmpty(nameof(name));

        foreach (var item in attrData.NamedArguments) if (item.Key == name) return item.Value;
        return null;
    }
}
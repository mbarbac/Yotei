namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Flattens into a single list the attributes found in the given collection of sources. No
    /// attempts are made to prevent any kind of duplications.
    /// </summary>
    /// <param name="sources"></param>
    /// <returns></returns>
    public static List<AttributeSyntax> GetAttributes(this SyntaxList<AttributeListSyntax> sources)
    {
        return sources.SelectMany(x => x.Attributes).ToList();
    }

    // -----------------------------------------------------

    /// <summary>
    /// Returns the list of attributes that decorates the given symbol whose attribute data class
    /// matches the given type.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<AttributeData> GetAttributeData(this ISymbol item, Type type)
    {
        item.ThrowWhenNull();
        type.ThrowWhenNull();

        var atds = item.GetAttributes();
        return atds.Where(x => x.Match(type)).ToList();
    }

    /// <summary>
    /// Determines if the given symbol is decorated by any attributes whose attribute data class
    /// match the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasAttributes(
        this ISymbol item, Type type) => item.GetAttributeData(type).Any();

    // -----------------------------------------------------

    /// <summary>
    /// Tries to obtain the value carried by the named argument whose name is given, in the
    /// given attribute data.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool GetNamedArgument(
        this AttributeData attribute,
        string name,
        [NotNullWhen(true)] out TypedConstant? item)
    {
        attribute.ThrowWhenNull();
        name = name.NotNullNotEmpty();

        foreach (var temp in attribute.NamedArguments)
        {
            if (temp.Key == name)
            {
                item = temp.Value;
                return true;
            }
        }

        item = null;
        return false;
    }
}
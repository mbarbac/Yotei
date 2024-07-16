namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class AttributeExtensions
{
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

    // -----------------------------------------------------

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
    /// Returns a list with the attributes applied to the given symbol that match the given type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<AttributeData> GetAttributes(this ISymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        return symbol.GetAttributes().Match(type);
    }

    /// <summary>
    /// Determines if the given symbol carries any attributes whose type is the given one.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasAttributes(this ISymbol symbol, Type type)
    {
        return symbol.GetAttributes(type).Any();
    }
}
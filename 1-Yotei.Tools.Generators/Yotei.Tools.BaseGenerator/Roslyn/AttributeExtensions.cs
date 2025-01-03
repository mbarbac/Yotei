namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns a flattened collection with the attributes found in the given collection of
    /// sources. No attempts are made to prevent any kind of duplications.
    /// </summary>
    /// <param name="sources"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeSyntax> GetAttributes(this SyntaxList<AttributeListSyntax> sources)
    {
        return sources.SelectMany(x => x.Attributes);
    }

    /// <summary>
    /// Returns the collection of attributes applied to the given symbols whose class type symbols
    /// match with the given regular type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, Type type)
    {
        symbol.ThrowWhenNull();
        type.ThrowWhenNull();

        return symbol.GetAttributes().SelectMatch(type);
    }

    /// <summary>
    /// Determines if the given symbol has any attribute applied to it whose class type symbol
    /// matches the given regular type.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasAttributes(this ISymbol symbol, Type type)
    {
        return symbol.GetAttributes(type).Any();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the value carried by the named argument whose name is given, in the
    /// given attribute data instance.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool GetNamedArgument(
        this AttributeData attribute, string name, [NotNullWhen(true)] out TypedConstant? item)
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
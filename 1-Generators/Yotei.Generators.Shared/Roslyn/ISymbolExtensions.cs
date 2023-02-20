namespace Yotei.Generators;

// ========================================================
internal static class ISymbolExtensions
{
    /// <summary>
    /// Returns the attributes with the given name that decorate the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attrName"></param>
    /// <returns></returns>
    public static ImmutableArray<AttributeData> GetAttributes(
        this ISymbol symbol,
        string attrName)
        => symbol.GetAttributes(attrName, out _);

    /// <summary>
    /// Returns the attributes with the given name that decorate the given symbol.
    /// If just one is found, then it is placed in the out argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attrName"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    public static ImmutableArray<AttributeData> GetAttributes(
        this ISymbol symbol,
        string attrName,
        out AttributeData unique)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));
        attrName = attrName.NotNullNotEmpty(nameof(attrName));

        var items = symbol
            .GetAttributes()
            .Where(x => x.AttributeClass != null && x.AttributeClass.Name == attrName)
            .ToImmutableArray();

        unique = items.Length == 1 ? items[0] : null!;
        return items;
    }

    /// <summary>
    /// Determines if the given symbol has at least one attribute with the given name.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attrName"></param>
    /// <returns></returns>
    public static bool HasAttribute(
        this ISymbol symbol,
        string attrName)
        => symbol.HasAttribute(attrName, out _);

    /// <summary>
    /// Determines if the given symbol has at least one attribute with the given name.
    /// If just one is found, then it is placed in the out argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attrName"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    public static bool HasAttribute(
        this ISymbol symbol,
        string attrName,
        out AttributeData unique)
        => symbol.GetAttributes(attrName, out unique).Length != 0;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the long name of the given symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string LongName(this ISymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));
        return symbol.ToString();
    }

    /// <summary>
    /// Returns the short name of the given symbol, defined as its last dot-separated part.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string ShortName(this ISymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        var name = symbol.LongName();
        var item = name.ToLastPart();
        return item;
    }


    /// <summary>
    /// Returns the fully qualified name of the given symbol, following C# rules.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string FullyQualifiedName(this ISymbol symbol)
    {
        symbol = symbol.ThrowIfNull(nameof(symbol));

        var list = new List<string>();
        ISymbol? node = symbol;

        while (node != null)
        {
            switch (node)
            {
                case INamedTypeSymbol item: Add(item); break;
                case INamespaceSymbol item: Add(item); break;
            }
            node = node.ContainingSymbol;
        }

        list.Reverse();
        return string.Join(".", list);

        /// <summary> Adds the the list the given name of the symbol, if it is not null...
        /// </summary>
        void Add(ISymbol symbol)
        {
            var name = GivenName(symbol).NullWhenEmpty();
            if (name != null) list.Add(name);
        }

        /// <summary> Gets the short name of the given symbol...
        /// </summary>
        static string GivenName(ISymbol symbol)
        {
            if (symbol is INamedTypeSymbol type && !type.IsNamespace && type.IsGenericType)
            {
                var sb = new StringBuilder();
                sb.Append($"{type.Name}<");

                for (int i = 0; i < type.TypeArguments.Length; i++)
                {
                    if (i != 0) sb.Append(", ");

                    var temp = type.TypeArguments[i];
                    sb.Append(temp.FullyQualifiedName());
                }

                sb.Append(">");
                return sb.ToString();
            }
            else return symbol.Name;
        }
    }
}
namespace Yotei.Generators
{
    // ====================================================
    public static class AttributeExtensions
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

        // ------------------------------------------------

        /// <summary>
        /// Returns the attributes with the given name that decorates the given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static ImmutableArray<AttributeData> GetAttributes(
            this ISymbol symbol,
            string attrName)
            => symbol.GetAttributes(attrName, out _);

        /// <summary>
        /// Returns the attributes with the given name that decorates the given symbol.
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
    }
}
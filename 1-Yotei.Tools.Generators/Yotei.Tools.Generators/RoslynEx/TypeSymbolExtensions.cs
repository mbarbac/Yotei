namespace Yotei.Tools.Generators;

// ========================================================
internal static class TypeSymbolExtensions
{
    /// <summary>
    /// Enumerates, in inheritance order, the named type symbols of its base types.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static IEnumerable<ITypeSymbol> AllBaseTypes(this ITypeSymbol symbol)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        while (true)
        {
            symbol = symbol.BaseType!;

            if (symbol != null && !symbol.IsNamespace) yield return symbol;
            else break;
        }
    }

    /// <summary>
    /// Determines if instance of the source type whose symbol is given can be assigned to
    /// instances of the target one, whose symbol is also given. This method validates both
    /// the base types of the source type as well as its interfaces.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsAssignableTo(this ITypeSymbol source, ITypeSymbol target)
    {
        source = source.ThrowWhenNull(nameof(source));
        target = target.ThrowWhenNull(nameof(target));

        // Same type...
        var comparer = SymbolEqualityComparer.Default;
        if (comparer.Equals(source, target)) return true;

        // Parent elements...
        if (source.AllBaseTypes().Any(x => comparer.Equals(target, x))) return true;
        if (source.AllInterfaces.Any(x => comparer.Equals(target, x))) return true;

        // Not found...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the C#-alike fully qualified name of the given type
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="addNullable"></param>
    /// <returns></returns>
    public static string FullyQualifiedName(this ITypeSymbol symbol, bool addNullable)
    {
        symbol = symbol.ThrowWhenNull(nameof(symbol));

        List<string> names = new();
        ISymbol? node = symbol;

        while (node != null)
        {
            switch (node)
            {
                case INamespaceSymbol item: AddToList(item); break;
                case INamedTypeSymbol item: AddToList(item); break;
            }
            node = node.ContainingSymbol;
        }

        names.Reverse();
        var name = string.Join(".", names);

        return (addNullable && symbol.NullableAnnotation == NullableAnnotation.Annotated)
            ? (name + "?")
            : name;

        /// <summary>
        /// Adds the name of the given symbol to the list, provided it is not null.
        /// </summary>
        void AddToList(ISymbol symbol)
        {
            var name = GetShortName(symbol).NullWhenEmpty();
            if (name != null) names.Add(name);
        }

        /// <summary>
        /// Gets the short name of the given type or namespace symbol.
        /// </summary>
        string? GetShortName(ISymbol symbol)
        {
            if (symbol is INamedTypeSymbol type && type.IsGenericType && !type.IsNamespace)
            {
                var sb = new StringBuilder();
                sb.Append($"{type.Name}<");

                for (int i = 0; i < type.TypeArguments.Length; i++)
                {
                    if (i != 0) sb.Append(", ");

                    var temp = type.TypeArguments[i];
                    var name = temp.FullyQualifiedName(addNullable);
                    sb.Append(name);
                }

                sb.Append(">");
                return sb.ToString();
            }
            return symbol.Name;
        }
    }
}
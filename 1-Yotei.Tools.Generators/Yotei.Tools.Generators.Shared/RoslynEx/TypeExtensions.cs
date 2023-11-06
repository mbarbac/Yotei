namespace Yotei.Tools.Generators.Shared;

// ========================================================
internal static class TypeExtensions
{
    /// <summary>
    /// Determines if the given type is an interface or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsInterface(
        this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy constructor found for the given type, or null if any. In strict mode,
    /// the type of the sole argument of the constructor must be the type itself. In not strict
    /// mode, is enough if the type can be assigned to that argument.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetCopyConstructor(this ITypeSymbol symbol, bool strict)
    {
        var methods = symbol.GetMembers().OfType<IMethodSymbol>().Where(x =>
            x.MethodKind == MethodKind.Constructor &&
            x.IsStatic == false &&
            x.Parameters.Length == 1)
            .ToDebugArray();

        var comparer = SymbolEqualityComparer.Default;
        return strict
            ? methods.FirstOrDefault(x => comparer.Equals(symbol, x.Parameters[0].Type))
            : methods.FirstOrDefault(x => symbol.IsAssignableTo(x.Parameters[0].Type));
    }

    // ----------------------------------------------------

    /// <summary>
    /// Enumerates, in inheritance order, the named type symbols of its base types.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static IEnumerable<ITypeSymbol> AllBaseTypes(this ITypeSymbol symbol)
    {
        while (true)
        {
            symbol = symbol.BaseType!;

            if (symbol != null && !symbol.IsNamespace) yield return symbol;
            else break;
        }
    }

    // ----------------------------------------------------

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
namespace Yotei.Tools.BaseGenerator;

// ========================================================
internal static class TypeExtensions
{
    /// <summary>
    /// Determines whether the given type symbol is an interface.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsInterface(
        this ITypeSymbol symbol) => symbol.ThrowWhenNull().TypeKind == TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a partial one or not.
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static bool IsPartial(
        this BaseTypeDeclarationSyntax syntax) =>
        syntax.ThrowWhenNull().Modifiers.Any(SyntaxKind.PartialKeyword);

    /// <summary>
    /// Determines if the given type is a partial one or not.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsPartial(this ITypeSymbol symbol)
    {
        var nodes = symbol.ThrowWhenNull().GetSyntaxNodes();
        
        if (nodes.Any(IsPartial)) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a record or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsRecord(this BaseTypeDeclarationSyntax type)
    {
        type.ThrowWhenNull();

        return type.Kind() is
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Enumerates, in inheritance order, all the base types of the given one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<ITypeSymbol> AllBaseTypes(this ITypeSymbol type)
    {
        type.ThrowWhenNull();
        ITypeSymbol? temp = type;

        while ((temp = temp.BaseType) != null)
            if (!temp.IsNamespace) yield return temp;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type can be assigned to the given target one, or not, using
    /// both the inheritance chain and the declared interfaces, recursively.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsAssignableTo(this ITypeSymbol source, ITypeSymbol target)
    {
        // Same type...
        var comparer = SymbolComparer.Default;
        if (comparer.Equals(source, target)) return true;

        // Parent elements...
        if (source.AllBaseTypes().Any(x => comparer.Equals(target, x))) return true;
        if (source.AllInterfaces.Any(x => comparer.Equals(target, x))) return true;

        // Not found...
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy constructor for the given type, or null if any. In the default strict mode,
    /// the type of the unique argument of the constructor must be the type itself. In the optional
    /// non-strict mode, a match is considered when that type can be assigned to the type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetCopyConstructor(this ITypeSymbol type, bool strict = true)
    {
        type.ThrowWhenNull();

        var methods = type.GetMembers().OfType<IMethodSymbol>().Where(x =>
            x.MethodKind == MethodKind.Constructor &&
            x.IsStatic == false &&
            x.Parameters.Length == 1)
            .ToDebugArray();

        var comparer = SymbolComparer.Default;

        var method = methods.FirstOrDefault(x => comparer.Equals(type, x.Parameters[0].Type));
        if (method == null && !strict)
            method = methods.FirstOrDefault(x => type.IsAssignableTo(x.Parameters[0].Type));

        return method;
    }
}
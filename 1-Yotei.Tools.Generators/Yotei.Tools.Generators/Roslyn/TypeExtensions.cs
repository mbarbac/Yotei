namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class TypeExtensions
{
    /// <summary>
    /// Determines if the given type is an interface or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsInterface(
        this ITypeSymbol type) => type.TypeKind == TypeKind.Interface;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a partial one or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPartial(this BaseTypeDeclarationSyntax type)
    {
        type.ThrowWhenNull();

        if (type.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given type is a partial one or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPartial(this ITypeSymbol type)
    {
        type.ThrowWhenNull();

        var nodes = type.GetSyntaxNodes();
        if (nodes.Any(x => x.IsPartial())) return true;
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
    /// Returns a copy constructor for the given type, or null if any, trying first strict mode
    /// and then the non-strict one.
    /// </summary>
    /// <returns></returns>
    public static IMethodSymbol? GetCopyConstructor(this ITypeSymbol type)
    {
        return
            type.GetCopyConstructor(true) ??
            type.GetCopyConstructor(false);
    }

    /// <summary>
    /// Returns a copy constructor for the given type, or null if any. In strict mode, the type
    /// of the unique argument of the constructor must be the type itself. In non-strict mode, a
    /// match is considered when that type can be assigned to the type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetCopyConstructor(this ITypeSymbol type, bool strict)
    {
        type.ThrowWhenNull();

        var methods = type.GetMembers().OfType<IMethodSymbol>().Where(x =>
            x.MethodKind == MethodKind.Constructor &&
            x.IsStatic == false &&
            x.Parameters.Length == 1)
            .ToDebugArray();

        var comparer = SymbolComparer.Default;
        return strict
            ? methods.FirstOrDefault(x => comparer.Equals(type, x.Parameters[0].Type))
            : methods.FirstOrDefault(x => type.IsAssignableTo(x.Parameters[0].Type));
    }
}
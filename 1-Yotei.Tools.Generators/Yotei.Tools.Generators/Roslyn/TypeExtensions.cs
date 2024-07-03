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
}
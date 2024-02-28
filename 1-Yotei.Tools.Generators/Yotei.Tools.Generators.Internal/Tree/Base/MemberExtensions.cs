namespace Yotei.Tools.Generators.Internal;

// ========================================================
internal static class MemberExtensions
{
    /// <summary>
    /// Determines if the given type is a partial one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPartial(this BaseTypeDeclarationSyntax type)
    {
        if (type.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))) return true;
        return false;
    }

    /// <summary>
    /// Determines if the given type is a partial one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPartial(this ITypeSymbol type)
    {
        var nodes = type.GetSyntaxNodes();
        foreach (var node in nodes) if (node.IsPartial()) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a supported one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSupported(this BaseTypeDeclarationSyntax type)
    {
        if (type.Kind() is
            SyntaxKind.InterfaceDeclaration or
            SyntaxKind.ClassDeclaration or
            SyntaxKind.StructDeclaration or
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration) return true;

        return false;
    }

    /// <summary>
    /// Determines if the given type is a supported one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSupported(this ITypeSymbol type)
    {
        if (type.TypeKind is
            TypeKind.Class or
            TypeKind.Struct or
            TypeKind.Interface) return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a record alike one.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsRecord(this BaseTypeDeclarationSyntax type)
    {
        if (type.Kind() is
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration) return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the property has a suitable getter.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasGetter(this IPropertySymbol symbol)
    {
        return symbol.GetMethod != null;
    }

    /// <summary>
    /// Determines if the property has a suitable setter or initer.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasSetterOrIniter(this IPropertySymbol symbol)
    {
        return symbol.SetMethod != null;
    }

    /// <summary>
    /// Determines if the property has a suitable setter that is not an initer.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasSetter(this IPropertySymbol symbol)
    {
        return symbol.SetMethod != null && !symbol.SetMethod.IsInitOnly;
    }

    /// <summary>
    /// Determines if the property has a suitable initer.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool HasIniter(this IPropertySymbol symbol)
    {
        return symbol.SetMethod != null && symbol.SetMethod.IsInitOnly;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given field is a writtable one.
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static bool IsWrittable(this IFieldSymbol field)
    {
        if (!field.IsConst && !field.IsReadOnly && !field.HasConstantValue) return true;
        return false;
    }
}
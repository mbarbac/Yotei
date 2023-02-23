namespace Yotei.Generators;

// ========================================================
internal static class NewItemInfo
{
    /// <summary>
    /// Determines if the given type is a nullable one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullable(ITypeSymbol type)
    {
        type = type.ThrowIfNull(nameof(type));

        return
            type.NullableAnnotation == NullableAnnotation.Annotated ||
            type.IsReferenceType;
    }

    /// <summary>
    /// Determines if the given type is a cloneable one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsCloneable(ITypeSymbol type)
    {
        type = type.ThrowIfNull(nameof(type));

        if (HasCloneableInterface(type)) return true;
        if (HasCloneMethod(type)) return true;
        if (HasCloneableAttributes(type)) return true;

        return false;
    }

    static bool HasCloneableInterface(ITypeSymbol type)
    {
        if (type.Name == nameof(ICloneable)) return true;

        foreach (var iface in type.Interfaces) if (HasCloneableInterface(iface)) return true;
        return false;
    }

    static bool HasCloneMethod(ITypeSymbol type)
    {
        if (type.GetMembers().OfType<IMethodSymbol>().Any(x =>
            x.Name == "Clone" &&
            x.Parameters.Length == 0 &&
            x.IsStatic == false))
            return true;

        return false;
    }

    static bool HasCloneableAttributes(ITypeSymbol type)
    {
        if (type.HasAttribute(CLONEABLE_MEMBER, out var data))
        {
            var ignore = data.GetNamedArgument(IGNORE_CLONE);
            if (ignore != null &&
                ignore.Value.Value is bool ignoreValue && ignoreValue) return false;

            var deep = data.GetNamedArgument(DEEP_CLONE);
            if (deep != null &&
                deep.Value.Value is bool deepValue && deepValue) return true;
        }

        return false;
    }

    static readonly string CLONEABLE_MEMBER = "CloneableMemberAttribute";
    static readonly string IGNORE_CLONE = "Ignore";
    static readonly string DEEP_CLONE = "Deep";
}
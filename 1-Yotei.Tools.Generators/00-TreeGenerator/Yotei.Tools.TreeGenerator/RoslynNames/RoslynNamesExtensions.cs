namespace Yotei.Tools.TreeGenerator;

// ========================================================
internal static partial class RoslynNames
{
    /// <summary>
    /// Determines if the source name can be substituted by a known keyword and, if so, returns
    /// it. If not, returns null.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static string? ToSpecialName(this ITypeSymbol source)
    {
        if (source is IArrayTypeSymbol array)
        {
            var type = array.ElementType;
            var str = Core(type);

            str = $"{str}[{new string(',', array.Rank - 1)}]";
            return str;
        }
        return Core(source);

        static string? Core(ITypeSymbol source) => source.SpecialType switch
        {
            SpecialType.System_Void => "void",
            SpecialType.System_Object => "object",
            SpecialType.System_String => "string",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Char => "char",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_Byte => "byte",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_Int16 => "short",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_Int32 => "int",
            SpecialType.System_UInt64 => "ulong",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_Decimal => "decimal",
            _ => null,
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a generic-alike one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsGenericAlike(
        this ITypeSymbol source) => source.TypeKind == TypeKind.TypeParameter;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the source type is a nullable wrapper, either a <see cref="Nullable{T}"/>
    /// or a <see cref="IsNullable{T}"/> one, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableWrapper(this ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 && (
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
        source.OriginalDefinition.Name == nameof(IsNullable<>));

    /// <summary>
    /// If the source type is a nullable wrapper, either a <see cref="Nullable{T}"/> or a
    /// <see cref="IsNullable{T}"/> one, returns the underlying type and sets the out argument
    /// to true. Otherwise, just returns the given source type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static INamedTypeSymbol UnwrapNullable(this INamedTypeSymbol source, out bool nullable)
    {
        if (source.IsNullableWrapper())
        {
            nullable = true;
            return (INamedTypeSymbol)source.TypeArguments[0];
        }

        nullable = false;
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is a nullable one either because it has been decorated
    /// with a '<see langword="?"/>' nullable annotation, or because it is decorated with either
    /// a <see cref="NullableAttribute"/> or with a <see cref="IsNullableAttribute"/> attribute.
    /// <br/> This method does not consider if the source type is a wrapper one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableByAnnotationOrAttribute(this ISymbol source)
    {
        // By using nullable annotations...
        switch (source)
        {
            case ITypeSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IParameterSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IPropertySymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IFieldSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
            case IEventSymbol item when (item.NullableAnnotation == NullableAnnotation.Annotated): return true;
        }

        // By using metadata attributes...
        var name = "System.Runtime.CompilerServices.NullableAttribute";
        var ats = source.GetAttributes().Where(x => x.AttributeClass?.ToDisplayString() == name);
        foreach (var at in ats)
        {
            var items = at.GetType().GetField("NullableFlags");
            if (items != null)
            {
                var value = items.GetValue(at);

                if ((value is byte b && b == 2) ||
                    (value is byte[] bs && bs.Length > 0 && bs[0] == 2))
                    return true;
            }
        }

        // Use wrapper workaround...
        if (ats.Any(x => x.AttributeClass?.Name == nameof(IsNullableAttribute))) return true;

        // Not nullable...
        return false;
    }
}
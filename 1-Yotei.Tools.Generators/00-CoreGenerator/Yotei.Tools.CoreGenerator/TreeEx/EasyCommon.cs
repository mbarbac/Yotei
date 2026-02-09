namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Determines if the given type is a nullable wrapper or not.
    /// </summary>
    public static bool IsNullableWrapper(this ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 && (
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
        source.OriginalDefinition.Name == nameof(IsNullable<>));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given symbol is either decorated with a nullable annotation, or with
    /// the <see cref="IsNullableAttribute"/>.
    /// </summary>
    public static bool IsNullableDecorated(this ISymbol source)
    {
        var annotated = source switch
        {
            ITypeSymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
            IPropertySymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
            IFieldSymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
            IEventSymbol item => item.NullableAnnotation == NullableAnnotation.Annotated,
            _ => false
        };
        if (annotated) return true;

        if (source.GetAttributes().Any(
            x => x.AttributeClass?.Name == nameof(NullableAttribute)))
            return true;

        if (source.GetAttributes().Any(
            x => x.AttributeClass?.Name == nameof(IsNullableAttribute)))
            return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the special-type string that correspond to the given type, or null if any.
    /// </summary>
    public static string? ToSpecialName(this INamedTypeSymbol source) => source.SpecialType switch
    {
        SpecialType.System_Object => "object",
        SpecialType.System_Void => "void",
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
        SpecialType.System_Decimal => "decimal",
        SpecialType.System_Single => "float",
        SpecialType.System_Double => "double",
        SpecialType.System_String => "string",
        _ => null,
    };

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the string that correspond to the given accesibility value, or null if any.
    /// </summary>
    /// <param name="value"></param>
    public static string? ToAccesibilityString(
        this Accessibility value, bool usePrivate = false) => value switch
        {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Private => usePrivate ? "private" : null,
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => null
        };
}
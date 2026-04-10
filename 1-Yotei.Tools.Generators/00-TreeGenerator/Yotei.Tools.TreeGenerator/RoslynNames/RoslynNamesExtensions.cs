using System.Reflection.Metadata;

namespace Yotei.Tools.Generators;

// ========================================================
public static partial class RoslynNamesExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string NULLABLE_ATTRIBUTE = "System.Runtime.CompilerServices.NullableAttribute";
    private const string READ_ONLY_ATTRIBUTE = "System.Runtime.CompilerServices.IsReadOnlyAttribute";

    // ----------------------------------------------------

    /// <summary>
    /// Returns the special name of the given type, if any, or null otherwise.
    /// </summary>
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
    /// Determines if the type is a generic 'T' one, for EasyName purposes only.
    /// </summary>
    internal static bool IsGenericAlike(this ITypeSymbol source)
    {
        return source.TypeKind == TypeKind.TypeParameter;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a nullable wrapper, or not.
    /// </summary>
    internal static bool IsNullableWrapper(this ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 && (
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
        source.OriginalDefinition.Name == nameof(IsNullable<>));

    /// <summary>
    /// If the given type is a nullable wrapper, returns the underlying type and sets the out
    /// argument to true. Otherwise returns the original type and sets the out argument to false.
    /// </summary>
    internal static INamedTypeSymbol UnwrapNullable(this INamedTypeSymbol source, out bool nullable)
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
    /// Determines if the given symbol has been identified as a nullable one because either it has
    /// been decorated with a '<see langword="?"/>' symbol, or because any nullable attribute (as
    /// <see cref="NullableAttribute"/> or <see cref="IsNullableAttribute"/>) has been used.
    /// <br/> This method DOES NOT take into consideration if the given symbol is a nullable wrapper
    /// type-alike one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotatedOrAttribute(this ISymbol source)
    {
        /* TODO: Validate annotated nullability rules from regular EasyNames.
         /// <para>
         /// When nullability is enabled, the generic 'T' arguments always appear as annotated ones,
         /// but this is NOT consistent with what happens with reference types (that they loose this
         /// information). For consistency reasons, when the type is a 'T' one we will request that
         /// is either wrapped (which is not checked by this method), or decorated with the custom
         /// <see cref="IsNullableAttribute"/> attribute, as expected for reference types.
         /// </para>
         if (source.FullName != null) // Preventing annotated generic 'T' types...
         {
            if (source.GetCustomAttributes(typeof(NullableAttribute), false)
                .Any(x => IsNullableEnabled((NullableAttribute)x)))
                return true;
         }
         */

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
        var ats = source.GetAttributes();

        foreach (var at in ats.Where(x => x.AttributeClass?.ToDisplayString() == NULLABLE_ATTRIBUTE))
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

        // By using custome attribute...
        if (ats.Any(x => x.AttributeClass?.Name == nameof(IsNullableAttribute))) return true;

        // Not nullable...
        return false;
    }

    // ----------------------------------------------------
    /* TODO: HasReadOnlyAttribute en Roslyn EasyNames
    /// <summary>
    /// Determines if the given element is decorated with a readonly attribute.
    /// </summary>
    internal static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == READ_ONLY_ATTRIBUTE);
    }*/
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Determines if the given type is a nullable wrapper or not.
    /// </summary>
    static bool IsNullableWrapper(ITypeSymbol source) =>
        source is INamedTypeSymbol named &&
        named.Arity == 1 && (
        source.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T ||
        source.OriginalDefinition.Name == nameof(IsNullable<>));

    /// <summary>
    /// Returns the special-type string that correspond to the given type, or null if any.
    /// </summary>
    static string? ToSpecialName(INamedTypeSymbol source) => source.SpecialType switch
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
}
namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class NamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol source)
    {
        /// <summary>
        /// Returns the special-type string that correspond to the given type, or null if any.
        /// </summary>
        public string? ToSpecialName() => source.SpecialType switch
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
}
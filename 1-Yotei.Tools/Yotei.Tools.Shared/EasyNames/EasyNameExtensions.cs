#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
static partial class EasyNameExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string READ_ONLY_ATTRIBUTE = "System.Runtime.CompilerServices.IsReadOnlyAttribute";

    // ----------------------------------------------------

    /// <summary>
    /// Returns the special name of the given type, if any, or null otherwise.
    /// </summary>
    internal static string? ToSpecialName(this Type source)
    {
        if (source.IsByRef) return Core(source.GetElementType() ?? source);
        if (source.IsArray) return Core(source.GetElementType() ?? source) + "[]";
        return Core(source);

        static string? Core(Type source) => source switch
        {
            Type t when t == typeof(void) => "void",
            Type t when t == typeof(object) => "object",
            Type t when t == typeof(string) => "string",
            Type t when t == typeof(bool) => "bool",
            Type t when t == typeof(char) => "char",
            Type t when t == typeof(byte) => "byte",
            Type t when t == typeof(sbyte) => "sbyte",
            Type t when t == typeof(short) => "short",
            Type t when t == typeof(ushort) => "ushort",
            Type t when t == typeof(int) => "int",
            Type t when t == typeof(uint) => "uint",
            Type t when t == typeof(long) => "long",
            Type t when t == typeof(ulong) => "ulong",
            Type t when t == typeof(float) => "float",
            Type t when t == typeof(double) => "double",
            Type t when t == typeof(decimal) => "decimal",
            _ => null
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a generic 'T' one, for EasyName purposes only.
    /// </summary>
    internal static bool IsGenericAlike(this Type source)
    {
        // Types such 'Predicate<T>' are not considered 'T'-alike ones...
        if (source.GetGenericArguments().Length > 0) return false;

        return source.FullName == null
            || source.IsGenericType
            || source.IsGenericParameter
            || source.IsGenericTypeDefinition
#if NET5_0_OR_GREATER
            || source.IsGenericTypeParameter
            || source.IsGenericMethodParameter
#endif
        ;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given element is decorated with a readonly attribute.
    /// </summary>
    internal static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == READ_ONLY_ATTRIBUTE);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a nullable wrapper, or not.
    /// </summary>
    internal static bool IsNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable`1") ||
        source.Name.StartsWith("IsNullable`1"));

    /// <summary>
    /// Determines if the given type has been annotated as nullable. This method verifies if it
    /// is decorated with a nullable-alike attribute, but not if if it a nullable wrapper.
    /// </summary>
    /// <remarks>
    /// When nullability is enabled, then generic 'T' arguments always appear as annotated, but
    /// this is NOT consistent with what happens with plain reference types, that loose that
    /// information. So, for consistency reasons, when the type is a 'T' one we'll request that
    /// is either wrapped (which is not covered by this method), or decorated with the custom
    /// <see cref="IsNullableAttribute"/> attribute, as expected for other reference types.
    /// </remarks>
    internal static bool IsNullableAnnotated(this Type source)
    {
        if (source.FullName != null) // Preventing annotated generic 'T' types...
        {
            if (source.GetCustomAttributes(typeof(NullableAttribute), false)
                .Any(x => IsEnabled((NullableAttribute)x)))
                return true;

            // Used to emulate the nullability API...
            static bool IsEnabled(NullableAttribute? at) =>
                at != null &&
                at.NullableFlags.Length == 1 &&
                at.NullableFlags[0] == 2;
        }

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        return false;
    }
}
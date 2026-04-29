namespace Yotei.Tools;

// ========================================================
static partial class EasyNameExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string READ_ONLY_ATTRIBUTE = "System.Runtime.CompilerServices.IsReadOnlyAttribute";

    // ----------------------------------------------------

    /// <summary>
    /// Returns the special name of the given type, if any, or null otherwise. When not null, the
    /// string is the naked representation of that type, without modifiers or nullable annotations.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? ToSpecialName(this Type source)
    {
        // Intercepting special cases...
        if (source.IsByRef)
        {
            return Core(source.GetElementType() ?? source);
        }
        if (source.IsArray || source.IsSZArray)
        {
            var type = source.GetElementType() ?? source;
            var str = Core(type);
            
            if (str != null)
            {
                var rank = source.GetArrayRank();
                str = $"{str}[{new string(',', rank - 1)}]";
                return str;
            }
        }
        if (source.IsNullableWrapper()) source = source.GetGenericArguments()[0];

        // Returning for the naked source type...
        return Core(source);

        // Gets the actual special name for the naked source type...
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
    /// Determines if the given element is decorated with a readonly attribute that is carried
    /// in its metadata.
    /// </summary>
    public static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == READ_ONLY_ATTRIBUTE);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a generic 'T' one, for EasyName purposes only.
    /// </summary>
    internal static bool IsGenericAlike(this Type source)
    {
        // Types such 'Predicate<T>' are not considered 'T'-alike ones...
        if (source.GetGenericArguments().Length > 0) return false;

        return source.FullName == null ||
            source.IsGenericType ||
            source.IsGenericParameter ||
            source.IsGenericTypeDefinition ||
            source.IsGenericTypeParameter ||
            source.IsGenericMethodParameter;
    }

#if NETSTANDARD2_0
    extension(Type source)
    {
        /// <summary>
        /// Determines if the type is a generic type parameter, or not.
        /// </summary>
        internal bool IsGenericTypeParameter
            => source.IsGenericParameter && source.DeclaringMethod == null;

        /// <summary>
        /// Determines if the type is a generic method parameter, or not.
        /// </summary>
        internal bool IsGenericMethodParameter
            => source.IsGenericParameter && source.DeclaringMethod != null;
    }
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a nullable wrapper, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable`1") ||
        source.Name.StartsWith("IsNullable`1"));

    /// <summary>
    /// Determines if the given attribute indicates that the decorated element is annotated.
    /// </summary>
    /// <param name="at"></param>
    /// <returns></returns>
    public static bool IsNullabilityEnabled(this NullableAttribute at) =>
        at.NullableFlags.Length > 0 &&
        at.NullableFlags[0] == 2;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has been annotated as a nullable one, either because its
    /// metadata carries a <see cref="NullableAttribute"/> attribute, or because it has been
    /// decorated with the custom <see cref="IsNullableAttribute"/> one.
    /// <br/> Reference types may not carry the metadata attribute despite being marked as nullables.
    /// <br/> For coherence reasons, generic 'T'-alike types are treated as conventional reference ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this Type source)
    {
        // All but T-alike types...
        if (source.FullName != null)
        {
            if (source.GetCustomAttributes(typeof(NullableAttribute), false)
                .Any(x => IsNullabilityEnabled((NullableAttribute)x)))
                return true;
        }

        // Finding custom attributes...
        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        // Default...
        return false;
    }

    // ----------------------------------------------------
}
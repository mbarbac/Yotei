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

        return source.FullName == null ||
            source.IsGenericType ||
            source.IsGenericParameter ||
            source.IsGenericTypeDefinition ||
            source.IsGenericTypeParameter ||
            source.IsGenericMethodParameter;
    }

#if YOTEI_TOOLS_GENERATORS
    extension(Type source)
    {
        /// <summary>
        /// Determines if the type is a generic type parameter, or not.
        /// </summary>
        public bool IsGenericTypeParameter
            => source.IsGenericParameter && source.DeclaringMethod == null;

        /// <summary>
        /// Determines if the type is a generic method parameter, or not.
        /// </summary>
        public bool IsGenericMethodParameter
            => source.IsGenericParameter && source.DeclaringMethod != null;
    }
#endif

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
    /// Determines if the given attribute indicates that the decorated element is annotated.
    /// </summary>
    /// <param name="at"></param>
    /// <returns></returns>
    internal static bool IsNullableEnabled(this NullableAttribute at) =>
        at.NullableFlags.Length > 0 &&
        at.NullableFlags[0] == 2;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has been annotated as nullable, by checking if it has been
    /// decorated with a nullable-alike attribute (but not if it is a nullable wrapper: it must
    /// be checked independently).
    /// <para>
    /// When nullability is enabled, the generic 'T' arguments always appear as annotated ones,
    /// but this is NOT consistent with what happens with reference types (that they loose this
    /// information). For consistency reasons, when the type is a 'T' one we will requeste that
    /// is either wrapped (which is not checked by this method), or decorated with the custom
    /// <see cref="IsNullableAttribute"/> attribute, as expected for reference types.
    /// </para>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this Type source)
    {
        if (source.FullName != null) // Preventing annotated generic 'T' types...
        {
            if (source.GetCustomAttributes(typeof(NullableAttribute), false)
                .Any(x => IsNullableEnabled((NullableAttribute)x)))
                return true;
        }

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        return false;
    }

    /// <summary>
    /// Determines if the given method has been annotated as nullable, by checking if it has been
    /// decorated with a nullable-alike attribute.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this MethodBase source)
    {
        if (source.GetCustomAttributes(typeof(NullableAttribute), false)
            .Any(x => IsNullableEnabled((NullableAttribute)x)))
            return true;

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        return false;
    }

    // ----------------------------------------------------

#if YOTEI_TOOLS_GENERATORS

    /// <summary>
    /// Determines if the given element has been annotated as nullable, by checking if it has been
    /// decorated with a nullable-alike attribute.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this ICustomAttributeProvider source)
    {
        if (source.GetCustomAttributes(typeof(NullableAttribute), false)
            .Any(x => IsNullableEnabled((NullableAttribute)x)))
            return true;

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        return false;
    }

#else

    /// <summary>
    /// Determines if the given element has been annotated as nullable, using the standard
    /// nullability API.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this ParameterInfo source)
    {
    var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    /// <summary>
    /// Determines if the given element has been annotated as nullable, using the standard
    /// nullability API.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this PropertyInfo source)
    {
    var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    /// <summary>
    /// Determines if the given element has been annotated as nullable, using the standard
    /// nullability API.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this FieldInfo source)
    {
    var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    /// <summary>
    /// Determines if the given element has been annotated as nullable, using the standard
    /// nullability API.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableAnnotated(this EventInfo source)
    {
    var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

#endif
}
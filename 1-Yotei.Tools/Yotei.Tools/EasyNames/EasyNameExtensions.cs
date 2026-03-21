namespace Yotei.Tools;

// ========================================================
// Extensions for 'EasyName' purposes only.
public static partial class EasyNameExtensions
{
    /// <summary>
    /// Determines if the source name can be substituted by a known keyword and, if so, returns
    /// it. If not, returns null;
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
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
    /// Determines if the type is a generic-alike one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsGenericAlike(this Type source) =>
        (!source.IsGenericType && source.FullName == null) ||
        source.IsGenericParameter ||
        source.IsGenericTypeParameter ||
        source.IsGenericMethodParameter;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a nullable wrapper, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable`1") ||
        source.Name.StartsWith("IsNullable`1"));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given attribute identifies the decorated element as nullable, or not.
    /// </summary>
    /// <param name="at"></param>
    /// <returns></returns>
    internal static bool IsNullableEnabled(this NullableAttribute? at) =>
        at != null &&
        at.NullableFlags.Length > 0 &&
        at.NullableFlags[0] == 2;

    /// <summary>
    /// Determines if the given source is decorated with a <see cref="IsNullableAttribute"/>, or
    /// with a <see cref="NullableAttribute"/> with nullability enabled.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool HasNullableEnabledAttribute(this ICustomAttributeProvider source)
    {
        if (source.GetCustomAttributes(typeof(NullableAttribute), false)
            .Any(x => ((NullableAttribute)x).IsNullableEnabled()))
            return true;

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines nullability using the standard API.
    /// </summary>
    internal static bool IsNullableByApi(this ParameterInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    /// <summary>
    /// Determines nullability using the standard API.
    /// </summary>
    internal static bool IsNullableByApi(this PropertyInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    /// <summary>
    /// Determines nullability using the standard API.
    /// </summary>
    internal static bool IsNullableByApi(this FieldInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    /// <summary>
    /// Determines nullability using the standard API.
    /// </summary>
    internal static bool IsNullableByApi(this EventInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given element is decorated with a readonly attribute.
    /// </summary>
    internal static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        var temp = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == temp);
    }
}
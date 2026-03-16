namespace Yotei.Tools;

// ========================================================
// Extensions for 'EasyName' purposes only.
public static partial class EasyNameExtensions
{
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
        source.Name.StartsWith("Nullable´1") ||
        source.Name.StartsWith("IsNullable´1"));

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

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false)
            .Any())
            return true;

        return false;
    }
}
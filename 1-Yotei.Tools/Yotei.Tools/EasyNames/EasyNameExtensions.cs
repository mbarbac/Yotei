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

    /// <summary>
    /// Determines if the type is a nullable wrapper, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static bool IsNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable´1") ||
        source.Name.StartsWith("IsNullable´1"));
}
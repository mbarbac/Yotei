#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

using System.ComponentModel;

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class EasyNameExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string ISREADONLYATTRIBUTE = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
    private const string SCOPEDREFATTRIBUTE = "System.Runtime.CompilerServices.ScopedRefAttribute";
    private const string REQUIRESLOCATIONATTRIBUTE = "System.Runtime.CompilerServices.RequiresLocationAttribute";

    // ----------------------------------------------------

    /// <summary>
    /// Retuns the special name associated to the given type, or <see langword="null"/> if any.
    /// <br/> By default, <paramref name="deep"/> is enabled, meaning that if the type is an array,
    /// a pointer, a by-ref or a nullable wrapper, its underlying type is used instead.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? ToSpecialName(this Type source, bool deep = true)
    {
        if (deep)
        {
            if (source.IsByRef) return Core(source.GetElementType() ?? source);
            if (source.IsArray) return Core(source.GetElementType() ?? source);
            if (source.IsNullableWrapper()) return Core(source.GetGenericArguments()[0]);
        }
        return Core(source);

        // Gets the actual special name form the given core type...
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
    /// Determines if the type is a CLR <see cref="Nullable{T}"/> nullable wrapper.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsCoreNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable`1"));

    /// <summary>
    /// Determines if the type is either a CLR <see cref="Nullable{T}"/> nullable wrapper, or an
    /// extended <see cref="IsNullable{T}"/> one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable`1") ||
        source.Name.StartsWith("IsNullable`1"));

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given <see cref="NullableAttribute"/> attribute enables nullability in
    /// the decorated element, or not.
    /// </summary>
    /// <remarks>
    /// The attribute is itself an 'internal sealed' one, not intended for source code usage.
    /// For this reason, this method must be kept internal because the parameter would be less
    /// accessible than the method itself.
    /// </remarks>
    internal static bool IsNullableAttributeEnabled(this NullableAttribute at) =>
        at.NullableFlags.Length > 0 &&
        at.NullableFlags[0] == 2;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given element is decorated with a read-only attribute or not.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == ISREADONLYATTRIBUTE);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type is a generic <c>T</c>-alike one, or not.
    /// <br/> This method is for EasyName purposes only.
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

#if NETSTANDARD2_0_
    extension(Type source)
    {
        /// <summary>
        /// Determines if the type is a generic type parameter, or not.
        /// </summary>
        internal bool IsGenericTypeParameter
            => source.IsGenericParameter && source.DeclaringMethod == null;
    }
#endif

#if NETSTANDARD2_0_
    extension(Type source)
    {
        /// <summary>
        /// Determines if the type is a generic method parameter, or not.
        /// </summary>
        internal bool IsGenericMethodParameter
            => source.IsGenericParameter && source.DeclaringMethod != null;
    }
#endif

    // ----------------------------------------------------
}
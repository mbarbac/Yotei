using System.Reflection.Emit;

namespace Yotei.Tools;

// ========================================================
public static class TypeExtensions
{
    /// <summary>
    /// Determines if the given type is a static one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStatic(this Type type)
    {
        type = type.ThrowWhenNull();

        return
           type.Attributes.HasFlag(TypeAttributes.Abstract) &&
           type.Attributes.HasFlag(TypeAttributes.Sealed);
    }

    /// <summary>
    /// Determines if the given type is a nullable one, or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullable(this Type type)
    {
        type = type.ThrowWhenNull();

        if (type.IsClass || type.IsInterface || type.IsArray) return true;

        if (type.IsGenericType)
        {
            var temp = type.GetGenericTypeDefinition();
            if (temp == typeof(Nullable<>)) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the given type is a compiler generated one or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompilerGenerated(this Type type)
    {
        type = type.ThrowWhenNull();

        return type.GetCustomAttributes(
            typeof(CompilerGeneratedAttribute), true)
            .Length != 0;
    }

    /// <summary>
    /// Determines if the given type is an anonymous one or not.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAnonymous(this Type type)
    {
        type = type.ThrowWhenNull();

        return
            type.IsCompilerGenerated() &&
            type.Name.Contains("Anonymous") &&
            type.Name.Contains("<>");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if an instance of this type can be assigned to an instance of the other one.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAssignableTo(this Type type, Type other)
    {
        type.ThrowWhenNull();
        other.ThrowWhenNull();

        return other.IsAssignableFrom(type);
    }
}
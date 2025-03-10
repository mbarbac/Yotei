﻿namespace Yotei.Tools;

// ========================================================
public static class TypeExtensions
{
    /// <summary>
    /// Determines if the type is a static one, or not.
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
            type.IsGenericType &&
            type.Namespace is null &&
            type.Name.Contains("Anonymous") &&
            type.Name.StartsWith("<>");
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
}